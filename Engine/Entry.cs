global using System;
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

	static readonly List<IGame> _games = new();

	public static void Run<T>() where T : IGame, new() {
		runGame( new T() );
	}

	public static void Run( Type t ) {
		var gameInstance = Activator.CreateInstance( t );
		if ( gameInstance is not IGame game ) return;

		runGame( game );
	}

	static void runGame( IGame game ) {
		bool isFirstGame = _games.Count == 0;
		if ( isFirstGame )
			Graphics = new OpenTk.GraphicsBackend();

		game.Start();
		_games.Add( game );

		if ( !isFirstGame ) return;

		Graphics.Window.DoUpdate = update;
		Graphics.Window.DoRender = draw;

		Graphics.RunLoop();

		shutdown();
	}

	static void update( float delta ) {
		foreach ( var game in _games )
			game.Tick();
	}

	static void draw() {
		foreach ( var game in _games )
			game.Draw();
	}

	static void shutdown() {
		for ( int i = _games.Count - 1; i >= 0; i-- ) {
			_games[i].Shutdown();
			_games.RemoveAt( i );
		}

		Graphics.Dispose();
	}
}
