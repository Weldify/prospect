namespace Prospect.Engine;

interface IGraphicsBackend : IDisposable {
	IWindow Window { get; }
	PolygonMode PolygonMode { get; set; }

	void RunLoop();

	void DrawThingamabob();
}
