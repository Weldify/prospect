namespace Prospect.Engine;

interface IGraphicsBackend : IDisposable {
	IWindow Window { get; }

	void RunLoop();
}
