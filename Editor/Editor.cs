using System;
using ImGuiNET;
using Prospect.Engine;

namespace Prospect.Editor;

class Editor : IGame {
	static void Main() => Entry.Run<Editor>();

	bool isGooeing = false;

	public void Start() {
		Window.Title = "Prospect Editor";
	}

	public void Tick() {

	}

	public void Draw() {
		ImGui.Begin( "Projects" );
		ImGui.Text( "Pooping" );

		ImGui.Begin( "pooppjects" );
		ImGui.Text( "Pooping" );
	}

	public void Shutdown() {

	}
}