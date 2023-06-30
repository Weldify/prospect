namespace Prospect.Engine;

interface IGraphicsBackend : IDisposable {
	IWindow Window { get; }
	PolygonMode PolygonMode { get; set; }
	MouseMode MouseMode { get; set; }

	Action<Key> KeyDown { get; set; }
	Action<Key> KeyUp { get; set; }
	Action<Vector2f> MouseMoved { get; set; }

	/// <summary>
	/// Is this backend ready to draw things, load models, etc
	/// </summary>
	bool IsReady { get; }

	Action OnLoad { set; }
	Action OnRender { set; }

	IModel LoadModel( string path, ITexture texture );
	ITexture LoadTexture( string path );

	void RunLoop();

	void DrawModel( Model model, Transform transform );
}
