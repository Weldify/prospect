using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using System.Drawing;

namespace Prospect.Engine.OpenGl;

class Window : IWindow, IDisposable {
	public string Title {
		get => _nativeWindow.Title;
		set => _nativeWindow.Title = value;
	}

	public Action<float>? DoUpdate { get; set; }
	public Action? DoRender { get; set; }

	public event Action Load = () => { };

	readonly Silk.NET.Windowing.IWindow _nativeWindow;

	// These will never be null in _nativeWindow events
	ImGuiController _imGuiController = null!;
	public GL Gl = null!;
	IInputContext _inputContext = null!;

	public Window() {
		WindowOptions options = WindowOptions.Default with {
			UpdatesPerSecond = 30,
			FramesPerSecond = 30,
			VSync = false,

			Title = "Prospect game"
		};

		_nativeWindow = Silk.NET.Windowing.Window.Create( options );

		_nativeWindow.Load += () => {
			_imGuiController = new(
				Gl = _nativeWindow.CreateOpenGL(),
				_nativeWindow,
				_inputContext = _nativeWindow.CreateInput()
			);

			Load.Invoke();
		};

		_nativeWindow.Closing += () => {
			_imGuiController?.Dispose();
			_inputContext?.Dispose();
			Gl?.Dispose();
		};

		_nativeWindow.Update += delta => {
			DoUpdate?.Invoke( (float)delta );
		};

		_nativeWindow.Render += delta => {
			_imGuiController.Update( (float)delta );

			Gl.ClearColor( Color.FromArgb( 255, 0, 0, 0 ) );
			Gl.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit );

			DoRender?.Invoke();
			_imGuiController.Render();
		};

		_nativeWindow.FramebufferResize += size => Gl.Viewport( size );
	}

	public void Run() => _nativeWindow.Run();

	public void Dispose() {
		_nativeWindow.Dispose();
	}
}
