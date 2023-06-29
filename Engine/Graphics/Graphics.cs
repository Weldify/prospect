namespace Prospect.Engine;

public static class Graphics {
	public static PolygonMode PolygonMode {
		get => Entry.Graphics.PolygonMode;
		set => Entry.Graphics.PolygonMode = value;
	}

	public static void DrawModel( Model model, Transform transform ) => Entry.Graphics.DrawModel( model, transform );
}
