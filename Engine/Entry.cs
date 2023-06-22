using System;
using System.Diagnostics;
using System.Numerics;
using System.Text;

using Veldrid;
using Veldrid.StartupUtilities;
using Veldrid.Sdl2;
using Veldrid.SPIRV;

using ImGuiNET;
using System.Collections.Generic;

namespace Prospect.Engine;

public static partial class Entry {
	public static Window MainWindow { get; private set; } = null!;

	static readonly List<IGame> _games = new();

	public static void Run<T>() where T : IGame, new() {
		// Game shadows other variables, indent it
		{
			T game = new();
			game.Start();

			_games.Add( game );
		}

		// First game to start is the main game
		if ( _games.Count != 1 ) return;

		MainWindow = new( new Vector2u( 100, 100 ), new( 512, 512 ), "MainWindow" );

		var stopwatch = Stopwatch.StartNew();
		float delta;

		while ( !MainWindow.IsClosed ) {
			delta = stopwatch.ElapsedTicks / (float)Stopwatch.Frequency;
			stopwatch.Restart();

			// Update all windows
			foreach ( var window in Window.All )
				window.Update( delta );

			if ( MainWindow.IsClosed ) break;

			foreach ( var game in _games ) {
				game.Tick();
				game.Draw();
			}

			foreach ( var window in Window.All )
				window.Draw();
		}

		freeResources();
	}

	static void freeResources() {
		for ( int i = _games.Count - 1; i >= 0; i-- ) {
			_games[i].Shutdown();
			_games.RemoveAt( i );
		}

		foreach ( var window in Window.All )
			window.Dispose();
	}
}