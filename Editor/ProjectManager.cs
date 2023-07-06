using ImGuiNET;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Prospect.Engine;
using System.IO;
using System.Numerics;

namespace Prospect.Editor;

partial class ProjectManager {
	public string ProjectPath { get; private set; } = "";
	public bool HasProject { get; private set; } = false;
	Project? _project;

	bool isValidProjectFile( string file ) => File.Exists( file ) && Path.GetExtension( file ) == ".proj";
	bool canHouseNewProject( string path ) => Directory.Exists( path ) && Directory.GetFiles( path ).Length == 0;

	public void Draw() {
		ImGui.Begin( "Project manager" );

		if ( _project is Project proj )
			drawProjectInfo( proj );
		else {
			ImGui.Text( "Drag and drop a .proj file to open an existing project" );
			ImGui.TextColored( new Vector4( 1, 1, 0, 1 ), "Or" );
			ImGui.Text( "Drag and drop an empty folder to create a new project" );
		}

		ImGui.End();
	}

	void drawProjectInfo( Project proj ) {
		ImGui.TextColored( new Vector4( 1f, 1f, 0f, 1f ), proj.Title );
		ImGui.Button( "Options" );
		ImGui.SameLine( 0f, 4f );

		drawExport();

		ImGui.SameLine( 0, 4 );

		if ( ImGui.Button( "Close" ) )
			closeProject();

		ImGui.Separator();
	}

	string[] _exportOptimizationLevels = new[] { "Debug", "Release" };
	string _chosenExportOptimizationLevel = "Debug";

	void drawExport() {
		// Open export popup when we click export
		if ( ImGui.Button( "Export" ) )
			ImGui.OpenPopup( "ExportPopup" );

		if ( ImGui.BeginPopup( "ExportPopup" ) ) {
			// Draw optimization level selection
			if ( ImGui.BeginCombo( "Optimization level", _chosenExportOptimizationLevel ) ) {
				foreach ( var level in _exportOptimizationLevels ) {
					var isSelected = level == _chosenExportOptimizationLevel;

					if ( ImGui.Selectable( level, isSelected ) )
						_chosenExportOptimizationLevel = level;

					if ( isSelected )
						ImGui.SetItemDefaultFocus();
				}

				ImGui.EndCombo();
			}

			if ( ImGui.Button( "Export" ) ) {
				ImGui.CloseCurrentPopup();
				exportProject();
			}

			ImGui.EndPopup();
		}
	}

	public Status CreateProject( string projectPath ) {
		if ( !canHouseNewProject( projectPath ) ) return Status.Fail();

		var title = Path.GetFileName( projectPath ) ?? throw new Exception( "Aurgghh" );

		var codePath = Path.Combine( projectPath, "code" );
		Directory.CreateDirectory( codePath );

		File.WriteAllText( Path.Combine( codePath, "Game.cs" ), _SAMPLE_GAME_CODE );

		var project = new Project() {
			Title = title
		};

		var projectFilePath = Path.Combine( projectPath, "project.proj" );
		project.Write( projectFilePath );

		OpenProject( projectFilePath );

		return Status.Ok();
	}

	public Status OpenProject( string filePath ) {
		if ( !isValidProjectFile( filePath ) ) return Status.Fail();

		closeProject();

		_project = Resources.GetOrCreate<Project>( filePath );
		ProjectPath = Directory.GetParent( filePath )?.FullName ?? throw new Exception( "HOw" );

		regenerateCsproj( _project );
		regenerateEditorConfig();

		HasProject = true;

		return Status.Ok();
	}

	void closeProject() {
		if ( _project is null ) return;

		var projectFilePath = Path.Combine( ProjectPath, "project.proj" );
		_project.Write( projectFilePath );

		_project = null;
		ProjectPath = "";

		HasProject = false;
	}

	void exportProject() {
		// Copy runtime to build folder
		copyDirectory( "../../../runtime/Release/net7.0", Path.Combine( ProjectPath, "export" ) );

		if ( compileProject().IsOk ) {
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

	Status compileProject() {
		if ( !Enum.TryParse( _chosenExportOptimizationLevel, out OptimizationLevel optimizationLevel ) )
			throw new Exception( "Failed to parse optimization level" );

		string[] sourceFiles = getProjectCsFilePaths();

		List<SyntaxTree> syntaxTrees = new();
		foreach ( string file in sourceFiles ) {
			string code = File.ReadAllText( file );
			SyntaxTree tree = CSharpSyntaxTree.ParseText( code );
			syntaxTrees.Add( tree );
		}

		if ( Directory.GetParent( typeof( object ).Assembly.Location )?.FullName is not string netCoreDir ) {
			Console.WriteLine( "Couldn't find NET core dir" );
			return Status.Fail();
		}

		List<MetadataReference> references = new() {
			MetadataReference.CreateFromFile( "Prospect.Engine.dll" ),
			MetadataReference.CreateFromFile( "ImGui.NET.dll" ),
			MetadataReference.CreateFromFile( "YamlDotNet.dll" ),
		};

		// The following references (most of them) are required for compilation to not shit itself!
		foreach ( var file in _systemReferences ) {
			references.Add( MetadataReference.CreateFromFile( Path.Combine( netCoreDir, file ) ) );
		}

		var options = new CSharpCompilationOptions( OutputKind.DynamicallyLinkedLibrary )
			.WithOptimizationLevel( optimizationLevel );

		var compilation = CSharpCompilation.Create(
			"game.dll",
			syntaxTrees,
			references,
			options
		);

		var exportDir = Path.Combine( ProjectPath, "export" );
		Directory.CreateDirectory( exportDir );

		var gameDllPath = Path.Combine( exportDir, "game.dll" );

		var result = compilation.Emit( gameDllPath );
		result.Diagnostics.ToList().ForEach( d => Console.WriteLine( d ) );

		// Success = good!
		if ( result.Success )
			return Status.Ok();

		// Clean up after fail
		File.Delete( gameDllPath );

		return Status.Fail();
	}

	string[] getProjectCsFilePaths() {
		var codeFolderPath = Path.Combine( ProjectPath, "code" );
		return Directory.GetFiles( codeFolderPath, "**.cs" );
	}

	void regenerateCsproj( Project project ) {
		var csprojPath = Path.Combine( ProjectPath, $"{project.Title}.csproj" );
		File.WriteAllText( csprojPath, generateCsprojContents() );
	}

	void regenerateEditorConfig() {
		var endFilePath = Path.Combine( ProjectPath, ".editorconfig" );

		// Currently I just yoink this from the engine, probably not a good idea
		var currentDir = Directory.GetCurrentDirectory();
		var editorConfigFile = Path.Combine( currentDir, "../../../../.editorconfig" );

		if ( !File.Exists( editorConfigFile ) ) {
			Console.WriteLine( "Didnt copy existing .editorconfig into project because we couldnt find it" );
			File.WriteAllText( "", endFilePath );
			return;
		}

		File.Copy( editorConfigFile, endFilePath, true );
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
	<Reference Include=""{Path.GetFullPath( "YamlDotNet.dll" )}"" />
  </ItemGroup>
</Project>
";
	}
}
