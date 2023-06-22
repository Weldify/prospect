using ImGuiNET;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Prospect.Engine;

class MainWindow : GameWindow {
	readonly ImGuiController _imGuiController;

	public MainWindow() : base(
		GameWindowSettings.Default, new() { WindowState = WindowState.Maximized, APIVersion = new( 3, 3 ), Vsync = VSyncMode.Off }
	) {
		_imGuiController = new( ClientSize.X, ClientSize.Y );
	}

	protected override void OnResize( ResizeEventArgs args ) {
		base.OnResize( args );

		GL.Viewport( 0, 0, ClientSize.X, ClientSize.Y );
		_imGuiController?.WindowResized( ClientSize.X, ClientSize.Y );
	}

	protected override void OnUpdateFrame( FrameEventArgs e ) {
		base.OnUpdateFrame( e );

		Entry.Update( (float)e.Time );
	}

	protected override void OnRenderFrame( FrameEventArgs e ) {
		base.OnRenderFrame( e );

		_imGuiController.Update( this, (float)e.Time );

		GL.ClearColor( new Color4( 0, 32, 48, 255 ) );
		GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit );

		Entry.Draw();

		ImGui.ShowDemoWindow();

		_imGuiController.Render();
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
}