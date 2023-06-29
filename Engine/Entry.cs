global using System;
global using System.Linq;
global using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;

using ImGuiNET;

namespace Prospect.Engine;

public static partial class Entry {
	internal static IGraphicsBackend Graphics { get; private set; }
	static IGame? _game;

	static Entry() {
		Graphics = new OpenGL.GraphicsBackend {
			OnLoad = onGraphicsLoaded,
			OnRender = render
		};
		Graphics.Window.DoUpdate = update;
	}

	public static void Run<T>() where T : IGame, new() {
		runGame( new T() );
	}

	public static void Run( Type t ) {
		var gameInstance = Activator.CreateInstance( t );
		if ( gameInstance is not IGame game ) return;

		runGame( game );
	}

	static void runGame( IGame game ) {
		_game = game;
		game.Start();

		Graphics.RunLoop();

		shutdown();
	}

	static void onGraphicsLoaded() {
		foreach ( var (_, model) in Model.Cache )
			(model as IPreloadable).Ready();
	}

	static void update( float delta ) {
		_game?.Tick();
	}

	static void render() {
		_game?.Render();
	}

	static void shutdown() {
		_game?.Shutdown();

		Graphics.Dispose();
	}
}
