using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;

namespace Prospect.Engine.OpenGL;

partial class GraphicsBackend : IGraphicsBackend {
	public bool HasLoaded { get; private set; } = false;

	public string WindowTitle {
		get => _window.Title;
		set => _window.Title = value;
	}

	readonly IWindow _window;
	ImGuiController? _imGuiController;

	// These wont be null when HasLoaded is true
	GL _gl = null!;
	Shader _mainModelShader = null!;
	IInputContext _inputContext = null!;

	public GraphicsBackend() {
		WindowOptions options = WindowOptions.Default with {
			FramesPerSecond = 165,
			VSync = false,

			Title = "Prospect game"
		};

		_window = Silk.NET.Windowing.Window.Create( options );
		_window.Load += onLoad;
		_window.Update += onUpdate;
		_window.Render += onRender;
		_window.FramebufferResize += onResize;
		_window.Closing += onClose;
	}

	public void RunLoop() {
		_window.Run();
	}

	void onLoad() {
		_gl = _window.CreateOpenGL();

		initInput();
		initRendering();

		_imGuiController = new( _gl, _window, _inputContext );

		HasLoaded = true;
		OnLoad.Invoke();
	}

	void onUpdate( double delta ) {
		OnUpdate.Invoke();
	}

	void onRender( double doubleDelta ) {
		var delta = (float)doubleDelta;

		beginRender( delta );
		OnRender.Invoke( delta );
		endRender();
	}

	void onResize( Vector2D<int> size ) {
		_gl.Viewport( size );
	}

	void onClose() {
		_imGuiController?.Dispose();
		_inputContext?.Dispose();
		_gl?.Dispose();
	}
}
