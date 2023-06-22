global using System;
global using System.Linq;
global using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp;
using System.Numerics;
using System.IO;

using ImGuiNET;
using Prospect.Engine;
using Microsoft.CodeAnalysis;

namespace Prospect.Editor;

class Editor : IGame {
	static void Main() => Entry.Run<Editor>();

	public void Start() {
		Window.Title = "Prospect Editor";

		var settings = Resources.GetOrCreate<EditorSettings>( "settings.eds" );
		_projectOpenPath = settings.LastProjectPath;
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
		settings.Write( "settings.eds" );
	}

	string _projectOpenPath = "";
	void drawOpenProject() {
		ImGui.SeparatorText( "Open" );
		ImGui.InputText( ".proj file", ref _projectOpenPath, 64 );

		var opening = ImGui.Button( "Open" );
		if ( !opening ) return;
		if ( !File.Exists( _projectOpenPath ) ) return;

		_currentProjectFilePath = _projectOpenPath;
		_currentProject = Resources.Get<Project>( _currentProjectFilePath );
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
		ImGui.TextColored( new Vector4( 1f, 1f, 0f, 1f ), proj.Title );
		ImGui.Button( "Options" );
		ImGui.SameLine( 0, 4 );
		ImGui.Button( "Run" );
		ImGui.SameLine( 0, 4 );

		if ( ImGui.Button( "Export" ) )
			exportProject();

		ImGui.SameLine( 0, 4 );

		if ( ImGui.Button( "Close" ) )
			closeProject();

		ImGui.Separator();
	}

	void createProject( string parentDir, string title ) {
		var projectPath = Path.Combine( parentDir, title );
		Directory.CreateDirectory( projectPath );

		var project = new Project() {
			Title = title,
		};

		var projectFilePath = Path.Combine( projectPath, "project.proj" );
		project.Write( projectFilePath );

		_currentProjectFilePath = projectFilePath;
		_currentProject = project;
	}

	void closeProject() {
		// Wasn't open
		if ( _currentProject is not Project proj ) return;

		proj.Write( _currentProjectFilePath );

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

		// I'm not sure why this is needed, but it is needed!
		var mscorlib =
					MetadataReference.CreateFromFile( typeof( object ).Assembly.Location );
		var codeAnalysis =
				MetadataReference.CreateFromFile( typeof( SyntaxTree ).Assembly.Location );
		var csharpCodeAnalysis =
				MetadataReference.CreateFromFile( typeof( CSharpSyntaxTree ).Assembly.Location );

		MetadataReference[] references = { mscorlib, codeAnalysis, csharpCodeAnalysis };

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
