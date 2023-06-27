using Silk.NET.Windowing;

namespace Prospect.Engine.OpenGl;

class Window : IWindow, IDisposable {
	public string Title {
		get => _nativeWindow.Title;
		set => _nativeWindow.Title = value;
	}

	public Action<float>? DoUpdate { get; set; }
	public Action? DoRender { get; set; }

	readonly Silk.NET.Windowing.IWindow _nativeWindow;

	public Window() {
		WindowOptions options = WindowOptions.Default with {
			Size = new( 800, 600 ),
			Title = "Prospect game"
		};

		_nativeWindow = Silk.NET.Windowing.Window.Create( options );
		_nativeWindow.Update += onUpdate;
		_nativeWindow.Render += onRender;
	}

	public void Run() => _nativeWindow.Run();

	void onUpdate( double delta ) {
		DoUpdate?.Invoke( (float)delta );
	}

	void onRender( double delta ) {
		DoRender?.Invoke();
	}

	public void Dispose() {
		_nativeWindow.Dispose();
	}
}
