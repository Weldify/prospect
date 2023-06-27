using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Prospect.Engine.OpenTk;

class Window : GameWindow, IWindow {
	readonly ImGuiController _imGuiController;

	public Action<float>? DoUpdate { get; set; }
	public Action? DoRender { get; set; }

	public Window() : base(
		GameWindowSettings.Default, new() { WindowState = WindowState.Maximized, APIVersion = new( 3, 3 ), Vsync = VSyncMode.Off }
	) {
		UpdateFrequency = 30;
		RenderFrequency = 30;

		_imGuiController = new( ClientSize.X, ClientSize.Y );
	}

	protected override void OnResize( ResizeEventArgs args ) {
		base.OnResize( args );

		GL.Viewport( 0, 0, ClientSize.X, ClientSize.Y );
		_imGuiController?.WindowResized( ClientSize.X, ClientSize.Y );
	}

	protected override void OnUpdateFrame( FrameEventArgs e ) {
		base.OnUpdateFrame( e );
		DoUpdate?.Invoke( (float)e.Time );
	}

	protected override void OnRenderFrame( FrameEventArgs e ) {
		base.OnRenderFrame( e );

		_imGuiController.Begin( this, (float)e.Time );

		GL.ClearColor( new Color4( 0, 0, 0, 255 ) );
		GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit );

		DoRender?.Invoke();

		_imGuiController.End();
		ImGuiController.CheckGLError( "End of frame" );

		SwapBuffers();
	}

	protected override void OnTextInput( TextInputEventArgs e ) {
		base.OnTextInput( e );
		_imGuiController.PressChar( (char)e.Unicode );
	}

	protected override void OnMouseWheel( MouseWheelEventArgs e ) {
		base.OnMouseWheel( e );
		_imGuiController.MouseScroll( e.Offset );
	}
};
