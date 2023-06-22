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

	string _currentProjectPath = "";

	public void Draw() {
		ImGui.Begin( "Project manager" );

		if ( Path.Exists( _currentProjectPath ) )
			drawOpenProject();
		else {
			drawProjectCreation();
		}

		ImGui.End();
	}

	string _creationProjectDir = "";

	void drawProjectCreation() {
		ImGui.InputText( "Location", ref _creationProjectDir, 64 );

		var pathExists = Path.Exists( _creationProjectDir )
			&& File.GetAttributes( _creationProjectDir ).HasFlag( FileAttributes.Directory );

		if ( pathExists ) {
			ImGui.Button( "Import" );
			return;
		}

		if (
			Path.GetPathRoot( _creationProjectDir ) is not string pathParent
			|| Path.GetDirectoryName( _creationProjectDir ) is not string projectName
		) return;

		ImGui.Button( "Create" );
	}

	void drawOpenProject() {
		ImGui.TextColored( new Vector4( 1f, 1f, 0f, 1f ), "Project name here" );
		ImGui.Button( "Options" );
		ImGui.SameLine( 0, 4 );
		ImGui.Button( "Run" );
		ImGui.SameLine( 0, 4 );
		ImGui.Button( "Export" );
		ImGui.SameLine( 0, 4 );
		ImGui.Button( "Close" );

		ImGui.Separator();
	}

	public void Shutdown() {

	}
}