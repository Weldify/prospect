namespace Prospect.Engine;

interface IGraphicsBackend : IDisposable {
	IWindow Window { get; }
	IInput Input { get; }
	PolygonMode PolygonMode { get; set; }

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
