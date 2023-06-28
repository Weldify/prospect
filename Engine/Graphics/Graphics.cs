namespace Prospect.Engine;

public static class Graphics {
	public static PolygonMode PolygonMode {
		get => Entry.Graphics.PolygonMode;
		set => Entry.Graphics.PolygonMode = value;
	}

	public static void DrawThingamabob() => Entry.Graphics.DrawThingamabob();
}
