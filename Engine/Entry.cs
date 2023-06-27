﻿global using System;
global using System.Linq;
global using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;

using ImGuiNET;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Prospect.Engine;

public static partial class Entry {
	internal static IGraphicsBackend Graphics { get; private set; } = null!;
	static IGame? _game;

	public static void Run<T>() where T : IGame, new() {
		runGame( new T() );
	}

	public static void Run( Type t ) {
		var gameInstance = Activator.CreateInstance( t );
		if ( gameInstance is not IGame game ) return;

		runGame( game );
	}

	static void runGame( IGame game ) {
		Graphics = new OpenGl.GraphicsBackend();

		Graphics.Window.DoUpdate = update;
		Graphics.Window.DoRender = render;

		_game = game;
		game.Start();

		Graphics.RunLoop();

		shutdown();
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
