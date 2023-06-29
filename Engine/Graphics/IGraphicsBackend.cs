namespace Prospect.Engine;

interface IGraphicsBackend : IDisposable {
	IWindow Window { get; }
	PolygonMode PolygonMode { get; set; }

	Action OnRender { set; }

	IModel LoadModel( string path, ITexture texture );
	ITexture LoadTexture( string path );

	void RunLoop();

	void DrawModel( Model model, Transform transform );
}
