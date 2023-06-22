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
		ImGui.Text( "I am gooey" );

		if ( ImGui.Button( "Are we gooing?" ) )
			isGooeing = !isGooeing;

		if ( isGooeing )
			ImGui.Text( "OO MAI GOT!!! AMGOOING@GGG" );
	}

	public void Shutdown() {

	}
}