global using System;
global using System.Linq;
global using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp;
using System.Numerics;
using System.IO;

using Prospect.Engine;
using Microsoft.CodeAnalysis;

namespace Prospect.Editor;

partial class Editor : IGame {
	static void Main() => Entry.Run<Editor>();

	public void Start() {
		Window.Title = "Prospect Editor";

		var settings = Resources.GetOrCreate<EditorSettings>( "settings.eds" );
		_projectFileOpenPath = settings.LastProjectPath;

		if ( !Path.Exists( _projectFileOpenPath ) )
			_projectFileOpenPath = "";

		if ( _projectFileOpenPath != "" )
			openProject( _projectFileOpenPath );
	}

	public void Tick() {

	}

	string _currentProjectFilePath = "";
	Project? _currentProject;

	public void Draw() {
		ImGui.Begin( "Project manager" );

		if ( Path.Exists( _currentProjectFilePath ) && _currentProject is Project proj )
			drawProjectInfo( proj );
		else {
			drawOpenProject();
			drawCreateProject();
		}

		ImGui.End();
	}

	public void Shutdown() {
		var settings = Resources.GetOrCreate<EditorSettings>( "settings.eds" );
		settings.LastProjectPath = _currentProjectFilePath;
		File.WriteAllText( "settings.eds", settings.Serialize() );
	}

	string _projectFileOpenPath = "";
	void drawOpenProject() {
		ImGui.SeparatorText( "Open" );
		ImGui.InputText( ".proj file", ref _projectFileOpenPath, 64 );

		var opening = ImGui.Button( "Open" );
		if ( !opening ) return;
		if ( !File.Exists( _projectFileOpenPath ) ) return;

		openProject( _projectFileOpenPath );
	}

	string _projectCreateDir = "";
	string _projectCreateName = "";
	void drawCreateProject() {
		ImGui.SeparatorText( "Create" );
		ImGui.InputText( "parent dir", ref _projectCreateDir, 64 );
		ImGui.InputText( "name", ref _projectCreateName, 64 );

		var creating = ImGui.Button( "Create" );
		if ( !creating ) return;

		// Is this root path valid
		if (
			!Path.Exists( _projectCreateDir )
			|| !File.GetAttributes( _projectCreateDir ).HasFlag( FileAttributes.Directory )
		) return;

		// Is the name free
		if (
			_projectCreateName == ""
			|| Path.Exists( Path.Combine( _projectCreateDir, _projectCreateName ) )
		) return;

		createProject( _projectCreateDir, _projectCreateName );
	}

	void drawProjectInfo( Project proj ) {
		ImGui.TextColored( proj.Title, new Vector4( 1f, 1f, 0f, 1f ) );
		ImGui.Button( "Options" );
		ImGui.SameLine( 0f, 4f );
		ImGui.Button( "Run" );
		ImGui.SameLine( 0f, 4f );

		if ( ImGui.Button( "Export" ) )
			exportProject();

		ImGui.SameLine( 0, 4 );

		if ( ImGui.Button( "Close" ) )
			closeProject();

		ImGui.Separator();
	}

	void createProject( string parentDir, string title ) {
		// Create main project path
		var projectPath = Path.Combine( parentDir, title );
		Directory.CreateDirectory( projectPath );

		// Create code path
		// Don't generate .csproj - it's generated while opening the project
		var codePath = Path.Combine( projectPath, "code" );
		Directory.CreateDirectory( codePath );

		// Create basic .cs file
		File.WriteAllText( Path.Combine( codePath, "Game.cs" ), SAMPLE_GAME_CODE );

		var project = new Project() {
			Title = title,
		};

		var projectFilePath = Path.Combine( projectPath, "project.proj" );
		File.WriteAllText( projectFilePath, project.Serialize() );

		openProject( projectFilePath );
	}

	void openProject( string projectFilePath ) {
		_currentProjectFilePath = projectFilePath;
		_currentProject = Resources.Get<Project>( _currentProjectFilePath );

		var projectPath = Directory.GetParent( projectFilePath )?.FullName ?? throw new Exception();

		// Regenerate .csproj file
		File.WriteAllText( Path.Combine( projectPath, $"{_currentProject.Title}.csproj" ), generateCsprojContents( _currentProject ) );
	}

	string generateCsprojContents( Project proj ) {
		return $@"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net7.0</TargetFramework>
	<ImplicitUsings>disable</ImplicitUsings>
	<Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <Reference Include=""{Path.GetFullPath( "Prospect.Engine.dll" )}"" />
  </ItemGroup>
</Project>
";
	}

	void closeProject() {
		// Wasn't open
		if ( _currentProject is not Project proj ) return;

		File.WriteAllText( _currentProjectFilePath, proj.Serialize() );

		_currentProject = null;
		_currentProjectFilePath = "";
	}

	void exportProject() {
		compileProject();
		Console.WriteLine( "Compiled" );
	}

	string[] getProjectCsFilePaths() {
		if ( Directory.GetParent( _currentProjectFilePath )?.FullName is not string projectPath ) throw new Exception();
		var codeFolderPath = Path.Combine( projectPath, "code" );

		return Directory.GetFiles( codeFolderPath, "**.cs" );
	}

	void compileProject() {
		string[] sourceFiles = getProjectCsFilePaths();

		List<SyntaxTree> syntaxTrees = new();
		foreach ( string file in sourceFiles ) {
			string code = File.ReadAllText( file );
			SyntaxTree tree = CSharpSyntaxTree.ParseText( code );
			syntaxTrees.Add( tree );
		}

		if ( Directory.GetParent( typeof( object ).Assembly.Location )?.FullName is not string netCoreDir )
			throw new Exception();

		// TODO: PLEASE PLEASE PLEASE future weldify get rid of the magic code
		List<MetadataReference> references = new() {
			MetadataReference.CreateFromFile( "Prospect.Engine.dll" )
		};

		// The following references (most of them) are required for compilation to not shit itself!
		foreach ( var file in _systemReferences ) {
			references.Add( MetadataReference.CreateFromFile( Path.Combine( netCoreDir, file ) ) );
		}

		var compilation = CSharpCompilation.Create(
			"game.dll",
			syntaxTrees,
			references,
			new( OutputKind.DynamicallyLinkedLibrary )
		);

		var projectDir = Directory.GetParent( _currentProjectFilePath )?.FullName ?? throw new Exception();
		var buildDir = Path.Combine( projectDir, "build" );
		Directory.CreateDirectory( buildDir );

		var result = compilation.Emit( Path.Combine( buildDir, "game.dll" ) );

		result.Diagnostics.ToList().ForEach( d => Console.WriteLine( d ) );
	}
}
