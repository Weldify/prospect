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
			//// Draw platform selection
			//if ( ImGui.BeginCombo( "Platform", _comboCurrentPlatform ) ) {
			//	foreach ( var platform in _comboPlatforms ) {
			//		var isSelected = platform == _comboCurrentPlatform;

			//		if ( ImGui.Selectable( platform, isSelected ) )
			//			_comboCurrentPlatform = platform;

			//		if ( isSelected )
			//			ImGui.SetItemDefaultFocus();
			//	}

			//	ImGui.EndCombo();
			//}

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
		// Copy runtime to build folder
		copyDirectory( "../../../runtime/Release/net7.0", Path.Combine( ProjectPath, "export" ) );

		if ( compileProject() ) {
			Console.WriteLine( "Compiled successfully!" );
		} else {
			Console.WriteLine( "Compilation failed" );
		}
	}

	void copyDirectory( string source, string destination ) {
		// Get information about the source directory
		var dir = new DirectoryInfo( source );

		// Check if the source directory exists
		if ( !dir.Exists )
			throw new DirectoryNotFoundException( $"Source directory not found: {dir.FullName}" );

		// Cache directories before we start copying
		DirectoryInfo[] dirs = dir.GetDirectories();

		// Create the destination directory
		Directory.CreateDirectory( destination );

		// Get the files in the source directory and copy to the destination directory
		foreach ( FileInfo file in dir.GetFiles() ) {
			string targetFilePath = Path.Combine( destination, file.Name );
			file.CopyTo( targetFilePath, true );
		}

		// If recursive and copying subdirectories, recursively call this method
		foreach ( DirectoryInfo subDir in dirs ) {
			string newDestinationDir = Path.Combine( destination, subDir.Name );
			copyDirectory( subDir.FullName, newDestinationDir );
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

		var exportDir = Path.Combine( ProjectPath, "export" );
		Directory.CreateDirectory( exportDir );

		var gameDllPath = Path.Combine( exportDir, "game.dll" );

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
