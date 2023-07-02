namespace Prospect.Engine;

public static class Window {
	public static string Title {
		get => Entry.Graphics.WindowTitle;
		set => Entry.Graphics.WindowTitle = value;
	}

	public static Action<string[]> OnFileDrop { internal get; set; } = ( p ) => { };
}
