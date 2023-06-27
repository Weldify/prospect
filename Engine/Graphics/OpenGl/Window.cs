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

	readonly Silk.NET.Windowing.IWindow _nativeWindow;

	// These will never be null in _nativeWindow events
	ImGuiController _imGuiController = null!;
	GL _gl = null!;
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
				_gl = _nativeWindow.CreateOpenGL(),
				_nativeWindow,
				_inputContext = _nativeWindow.CreateInput()
			);
		};

		_nativeWindow.Closing += () => {
			_imGuiController?.Dispose();
			_inputContext?.Dispose();
			_gl?.Dispose();
		};

		_nativeWindow.Update += delta => {
			DoUpdate?.Invoke( (float)delta );
		};

		_nativeWindow.Render += delta => {
			_imGuiController.Update( (float)delta );

			_gl.ClearColor( Color.FromArgb( 255, 0, 0, 0 ) );
			_gl.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit );

			DoRender?.Invoke();
			_imGuiController.Render();
		};

		_nativeWindow.FramebufferResize += size => _gl.Viewport( size );
	}

	public void Run() => _nativeWindow.Run();

	public void Dispose() {
		_nativeWindow.Dispose();
	}
}
