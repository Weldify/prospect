using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using System;
using System.Drawing;

namespace Prospect.Engine.OpenGL;

class Window : IDisposable {
	public string Title {
		get => _nativeWindow.Title;
		set => _nativeWindow.Title = value;
	}

	public Point2 Size {
		get => new( _nativeWindow.Size.X, _nativeWindow.Size.Y );
		set => _nativeWindow.Size = new( value.X, value.Y );
	}

	public Action OnLoad { get; set; } = () => { };
	public Action OnUpdate { get; set; } = () => { };
	public Action<float> OnRender { get; set; } = ( f ) => { };

	public GL GL => _gl ?? throw new Exception( "Window hasn't loaded yet" );

	readonly IWindow _nativeWindow;
	ImGuiController _imGuiController = null!;
	IInputContext _inputContext = null!;
	GL _gl = null!;

	public Window() {
		WindowOptions options = WindowOptions.Default with {
			FramesPerSecond = 165,
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

			OnLoad.Invoke();
		};

		_nativeWindow.Update += delta => {
			OnUpdate.Invoke();
		};

		_nativeWindow.Render += delta => {
			_imGuiController.Update( (float)delta );
			OnRender.Invoke( (float)delta );
			_imGuiController.Render();
		};

		_nativeWindow.Closing += () => {
			_imGuiController?.Dispose();
			_inputContext?.Dispose();
			_gl?.Dispose();
		};

		_nativeWindow.FramebufferResize += size => _gl.Viewport( size );
	}

	public IInputContext CreateInput() => _nativeWindow.CreateInput();
	public void RunLoop() => _nativeWindow.Run();

	public void Dispose() {
		_nativeWindow.Dispose();
	}
}
