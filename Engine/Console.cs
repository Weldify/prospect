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

	static readonly List<OutputEntry> _entries = new( new OutputEntry[_MAX_OUTPUT_SIZE] ) { };

	static bool _isOpen = false;
	static string _commandInput = "";

	static Consoole() {
		for ( var i = 1; i < _MAX_OUTPUT_SIZE; i++ ) {
			_entries[i] = new();
		}
	}

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
		if ( !ImGui.BeginTable( "__consoleOutput", 1 ) ) return;

		ImGui.TableSetupColumn( "Text", ImGuiTableColumnFlags.WidthStretch, 1f );

		foreach ( var entry in _entries ) {
			ImGui.TableNextRow();
			ImGui.TableNextColumn();

			ImGui.Text( entry.Message ?? "" );
		}

		ImGui.EndTable();
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