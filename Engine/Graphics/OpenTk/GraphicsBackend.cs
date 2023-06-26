namespace Prospect.Engine.OpenTk;

class GraphicsBackend : IGraphicsBackend {
	public IWindow Window => _window;
	readonly Window _window = new();

	public GraphicsBackend() {

	}

	public void RunLoop() {
		_window.Run();
	}

	public void Dispose() {
		_window.Dispose();
	}
}
