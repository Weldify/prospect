using ImGuiNET;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Prospect.Engine;
using System.IO;
using System.Numerics;
using Microsoft.CodeAnalysis.Emit;

namespace Prospect.Editor;

partial class ProjectManager {
	public string ProjectPath { get; private set; } = "";

	Project? _project;

	string _projectOpenPath = "";
	string _projectCreateDir = "";
	string _projectCreateName = "";

	string[] _comboPlatforms = new[] { "win-x64", "linux-x64" };
	string _comboCurrentPlatform = "";

	public ProjectManager() {
		_comboCurrentPlatform = _comboPlatforms[0];
	}

	public void TryRestoreProject( string path ) {
		if ( !Directory.Exists( path ) ) return;

		openProject( path );
	}

	public void Draw() {
		ImGui.Begin( "Project manager" );

		if ( _project is Project proj )
			drawProjectInfo( proj );
		else {
			drawOpenProject();
			drawCreateProject();
		}

		ImGui.End();
	}

	void drawOpenProject() {
		ImGui.SeparatorText( "Open" );
		ImGui.InputText( ".proj file", ref _projectOpenPath, 64 );

		var opening = ImGui.Button( "Open" );
		if ( !opening ) return;
		if ( !Directory.Exists( _projectOpenPath ) ) return;

		openProject( _projectOpenPath );
	}

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
		ImGui.TextColored( new Vector4( 1f, 1f, 0f, 1f ), proj.Title );
		ImGui.Button( "Options" );
		ImGui.SameLine( 0f, 4f );
		ImGui.Button( "Run" );
		ImGui.SameLine( 0f, 4f );

		drawExport();

		ImGui.SameLine( 0, 4 );

		if ( ImGui.Button( "Close" ) )
			closeProject();

		ImGui.Separator();
	}

	void drawExport() {
		// Open export popup when we click export
		if ( ImGui.Button( "Export" ) )
			ImGui.OpenPopup( "ExportPopup" );

		if ( ImGui.BeginPopup( "ExportPopup" ) ) {
			// Draw platform selection
			if ( ImGui.BeginCombo( "Platform", _comboCurrentPlatform ) ) {
				foreach ( var platform in _comboPlatforms ) {
					var isSelected = platform == _comboCurrentPlatform;

					if ( ImGui.Selectable( platform, isSelected ) )
						_comboCurrentPlatform = platform;

					if ( isSelected )
						ImGui.SetItemDefaultFocus();
				}

				ImGui.EndCombo();
			}

			if ( ImGui.Button( "Export" ) )
				exportProject();

			ImGui.EndPopup();
		}
	}

	void createProject( string parentPath, string title ) {
		var projectPath = Path.Combine( parentPath, title );
		Directory.CreateDirectory( projectPath );

		var codePath = Path.Combine( projectPath, "code" );
		Directory.CreateDirectory( codePath );

		File.WriteAllText( Path.Combine( codePath, "Game.cs" ), SAMPLE_GAME_CODE );

		var project = new Project() {
			Title = title
		};

		var projectFilePath = Path.Combine( projectPath, "project.proj" );
		project.Write( projectFilePath );

		openProject( projectPath );
	}

	void openProject( string path ) {
		var projectFilePath = Path.Combine( path, "project.proj" );

		_project = Resources.GetOrCreate<Project>( projectFilePath );
		ProjectPath = path;

		regenerateCsproj();
	}

	void closeProject() {
		if ( _project is null ) return;

		var projectFilePath = Path.Combine( ProjectPath, "project.proj" );
		_project.Write( projectFilePath );

		_projectOpenPath = ProjectPath;

		_project = null;
		ProjectPath = "";
	}

	void exportProject() {
		if ( compileProject() ) {
			Console.WriteLine( "Compiled successfully!" );
		} else {
			Console.WriteLine( "Compilation failed" );
		}
	}

	bool compileProject() {
		string[] sourceFiles = getProjectCsFilePaths();

		List<SyntaxTree> syntaxTrees = new();
		foreach ( string file in sourceFiles ) {
			string code = File.ReadAllText( file );
			SyntaxTree tree = CSharpSyntaxTree.ParseText( code );
			syntaxTrees.Add( tree );
		}

		if ( Directory.GetParent( typeof( object ).Assembly.Location )?.FullName is not string netCoreDir ) {
			Console.WriteLine( "Couldn't find NET core dir" );
			return false;
		}

		// TODO: PLEASE PLEASE PLEASE future weldify get rid of the magic code
		List<MetadataReference> references = new() {
			MetadataReference.CreateFromFile( "Prospect.Engine.dll" ),
			MetadataReference.CreateFromFile( "ImGui.NET.dll" )
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

		var buildDir = Path.Combine( ProjectPath, "build" );
		Directory.CreateDirectory( buildDir );

		var gameDllPath = Path.Combine( buildDir, "game.dll" );

		var result = compilation.Emit( gameDllPath );
		result.Diagnostics.ToList().ForEach( d => Console.WriteLine( d ) );

		// Success = good!
		if ( result.Success )
			return true;

		// Clean up after fail
		File.Delete( gameDllPath );

		return false;
	}

	string[] getProjectCsFilePaths() {
		var codeFolderPath = Path.Combine( ProjectPath, "code" );
		return Directory.GetFiles( codeFolderPath, "**.cs" );
	}

	void regenerateCsproj() {
		if ( _project is null ) return;

		var csprojPath = Path.Combine( ProjectPath, $"{_project.Title}.csproj" );
		File.WriteAllText( csprojPath, generateCsprojContents() );
	}

	string generateCsprojContents() {
		return $@"
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net7.0</TargetFramework>
	<ImplicitUsings>disable</ImplicitUsings>
	<Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <Reference Include=""{Path.GetFullPath( "Prospect.Engine.dll" )}"" />
	<Reference Include=""{Path.GetFullPath( "ImGui.NET.dll" )}"" />
  </ItemGroup>
</Project>
";
	}
}
