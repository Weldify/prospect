using System;
using System.Collections.Generic;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Prospect.Engine;

public sealed class Window : IDisposable {
	internal static IReadOnlyList<Window> All => new List<Window>( _all );
	static readonly List<Window> _all = new();

	public Vector2u Position { get; private set; }
	public Vector2u Size { get; private set; }
	public string Title { get; private set; }
	public bool IsClosed { get; private set; } = false;

	readonly Sdl2Window _nativeWindow;
	readonly GraphicsDevice _graphicsDevice;
	readonly ImGuiController _imGuiController;
	readonly CommandList _commandList;

	public Window( Vector2u position, Vector2u size, string title ) {
		Position = position;
		Size = size;
		Title = title;

		WindowCreateInfo windowCreateInfo = new() {
			X = (int)position.X,
			Y = (int)position.Y,

			WindowWidth = (int)size.X,
			WindowHeight = (int)size.Y,

			WindowTitle = title
		};

		_nativeWindow = VeldridStartup.CreateWindow( windowCreateInfo );

		GraphicsDeviceOptions graphicsDeviceOptions = new() {
			PreferDepthRangeZeroToOne = true,
			PreferStandardClipSpaceYDirection = true
		};

		_graphicsDevice = VeldridStartup.CreateGraphicsDevice( _nativeWindow, graphicsDeviceOptions, GraphicsBackend.Vulkan );

		_imGuiController = new( _graphicsDevice, Size );
		_commandList = _graphicsDevice.ResourceFactory.CreateCommandList();

		_all.Add( this );
	}

	internal void Update( float delta ) {
		InputSnapshot inputSnapshot = _nativeWindow.PumpEvents();
		IsClosed = !_nativeWindow.Exists;

		if ( IsClosed ) return;

		_imGuiController.Update( delta, inputSnapshot );
	}

	internal void Draw() {
		_commandList.Begin();
		_commandList.SetFramebuffer( _graphicsDevice.SwapchainFramebuffer );

		_commandList.ClearColorTarget( 0, RgbaFloat.Black );

		_imGuiController.Draw( _commandList );

		_commandList.End();

		_graphicsDevice.SubmitCommands( _commandList );
		_graphicsDevice.SwapBuffers();
	}

	public void Dispose() {
		_imGuiController.Dispose();
		_nativeWindow.Close();
		_graphicsDevice.Dispose();

		_all.Remove( this );
	}
}