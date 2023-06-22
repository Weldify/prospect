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
	internal static MainWindow Window = null!;
	static readonly List<IGame> _games = new();

	public static void Run<T>() where T : IGame, new() {
		bool isFirstGame = _games.Count == 0;
		if ( isFirstGame )
			Window = new() { UpdateFrequency = 30, RenderFrequency = 30 };

		T game = new();

		game.Start();
		_games.Add( game );

		if ( !isFirstGame ) return;

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