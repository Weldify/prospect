namespace Prospect.Engine;

public static class Window {
	public static string Title {
		get => Entry.Window.Title;
		set => Entry.Window.Title = value;
	}
}