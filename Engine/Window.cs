using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Prospect.Engine;

public sealed class Window {
	public Vector2u Position { get; private set; }
	public Vector2u Size { get; private set; }
	public string Title { get; private set; }

	readonly Sdl2Window _nativeWindow;
	readonly GraphicsDevice _graphicsDevice;

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
	}
}