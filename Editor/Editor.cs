global using System;
global using System.Linq;
global using System.Collections.Generic;

using System.Numerics;
using System.IO;

using ImGuiNET;
using Prospect.Engine;

namespace Prospect.Editor;

class Editor : IGame {
	static void Main() => Entry.Run<Editor>();

	public void Start() {
		Window.Title = "Prospect Editor";
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
		ImGui.Button( "Export" );
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
}