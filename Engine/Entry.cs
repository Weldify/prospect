using System;
using System.Diagnostics;
using System.Numerics;
using System.Text;

using ImGuiNET;
using System.Collections.Generic;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Prospect.Engine;

public static partial class Entry {
	internal static Window Window = null!;
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

		Window = new() { UpdateFrequency = 10, RenderFrequency = 10 };
		Window.Run();

		shutdown();
	}

	internal static void Update( float delta ) {
		foreach ( var game in _games )
			game.Tick();
	}

	internal static void Draw() {
		foreach ( var game in _games )
			game.Draw();
	}

	static void shutdown() {
		for ( int i = _games.Count - 1; i >= 0; i-- ) {
			_games[i].Shutdown();
			_games.RemoveAt( i );
		}
	}
}