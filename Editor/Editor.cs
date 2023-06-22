global using System;
global using System.Linq;
global using System.Collections.Generic;

using ImGuiNET;
using Prospect.Engine;

namespace Prospect.Editor;

class Editor : IGame {
	static void Main() => Entry.Run<Editor>();

	public void Start() {
		Window.Title = "Prospect Editor";

		var editorSettings = Resources.GetOrCreate<EditorSettings>( "settings.eds" );
		editorSettings.Projects.Add( "sex game" );

		editorSettings.Write( "settings.eds" );
	}

	public void Tick() {
		
	}

	public void Draw() {
		if ( ImGui.Begin( "Projects" ) ) {

		}
	}

	public void Shutdown() {

	}
}