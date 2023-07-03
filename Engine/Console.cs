using ImGuiNET;

namespace Prospect.Engine;

public static class Consoole {
	const uint _MAX_COMMAND_SIZE = 128;
	const uint _MAX_OUTPUT_SIZE = 5;

	/// <summary> Should the console be visible? </summary>
	public static bool IsOpen {
		get => _isOpen;
		set => _isOpen = value;
	}

	static readonly List<OutputEntry> _entries = new( new OutputEntry[_MAX_OUTPUT_SIZE] ) {
		new() { Message="Bigballs" }
	};

	static bool _isOpen = false;
	static string _commandInput = "";

	internal static void Frame() {
		if ( !_isOpen ) return;

		if ( ImGui.Begin( "Console", ref _isOpen ) )
			drawConsoleContents();

		ImGui.End();
	}

	static void drawConsoleContents() {
		drawOutput();
		ImGui.Separator();
		drawCommandInput();
	}

	static void drawOutput() {
		for ( int i = 0; i < _entries.Count; i++ ) {
			var entry = _entries[i];
			var message = entry.Message ?? "";

            ImGui.PushID(i);
			ImGui.InputText( "", ref message, byte.MaxValue, ImGuiInputTextFlags.ReadOnly );
            ImGui.PopID();
		}
	}

	static void drawCommandInput() {
		if ( ImGui.InputText( "", ref _commandInput, _MAX_COMMAND_SIZE, ImGuiInputTextFlags.EnterReturnsTrue ) ) {
			// Do something
		}
	}

	struct OutputEntry {
		public string Message;
	}
}