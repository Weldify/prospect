namespace Prospect.Engine;

interface IGraphicsBackend : IDisposable {
	IWindow Window { get; }
	PolygonMode PolygonMode { get; set; }

	Action OnRender { set; }

	void RunLoop();

	void DrawThingamabob();
}
