global using System;
global using System.Linq;
global using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;

using ImGuiNET;

namespace Prospect.Engine;

public static partial class Entry {
	public static uint TickRate = 0;
	public static float TickDelta = 0f;

	internal static IGraphicsBackend Graphics { get; private set; }
	static IGame? _game;

	internal static Stopwatch RawGameTime { get; private set; } = new();
	internal static uint CurrentTick { get; private set; } = 0;

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
		applyOptions( game );

		RawGameTime.Start();

		_game = game;
		game.Start();

		Graphics.RunLoop();

		shutdown();
	}

	static void applyOptions( IGame game ) {
		TickRate = game.Options.TickRate;
		TickDelta = 1f / (float)TickRate;
	}

	static void onGraphicsLoaded() {
		foreach ( var (_, model) in Model.Cache )
			(model as IPreloadable).Ready();
	}

	static void update( float delta ) {
		var expectedCurrentTick = Time.CalculateCurrentTick();

		while ( CurrentTick < expectedCurrentTick ) {
			CurrentTick++;
			_game?.Tick();
		}
	}

	static void render() {
		_game?.Render();
	}

	static void shutdown() {
		_game?.Shutdown();

		Graphics.Dispose();
	}
}
