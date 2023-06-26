namespace Prospect.Engine;

public static class Window {
	public static string Title {
		get => Entry.Graphics.Window.Title;
		set => Entry.Graphics.Window.Title = value;
	}
}
