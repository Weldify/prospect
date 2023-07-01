global using System;
global using System.Linq;
global using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using ImGuiNET;

namespace Prospect.Engine;

public static partial class Entry {
	public static uint TickRate { get; private set; } = 0;
	public static float TickDelta { get; private set; } = 0f;

	internal static IGraphicsBackend Graphics { get; private set; }

	internal static Stopwatch RawGameTime { get; private set; } = new();
	internal static uint CurrentTick { get; private set; } = 0;

	internal static Angles LookDelta { get; private set; } = Angles.Zero;
	internal static HashSet<Key> HeldKeys { get; private set; } = new();
	internal static HashSet<Key> PreviousHeldKeys { get; private set; } = new();

	static IGame? _game;
	static bool _hasGameStarted = false;

	static Entry() {
		Graphics = new OpenGL.GraphicsBackend {
			OnLoad = onGraphicsLoaded,
			OnRender = render
		};
		Graphics.Window.DoUpdate = update;

		Graphics.KeyDown = onKeyDown;
		Graphics.KeyUp = onKeyUp;
		Graphics.MouseMoved = onMouseMoved;
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
		tryStartGame();

		Graphics.RunLoop();

		shutdown();
	}

	static void tryStartGame() {
		if ( _game is null || !Graphics.IsReady || _hasGameStarted ) return;
		_hasGameStarted = true;
		_game.Start();
	}

	static void applyOptions( IGame game ) {
		TickRate = game.Options.TickRate;
		TickDelta = 1f / (float)TickRate;
	}

	static void onGraphicsLoaded() {
		foreach ( var (_, model) in Model.Cache )
			(model as IPreloadable).Ready();

		tryStartGame();
	}

	static void update( float delta ) {
		var expectedCurrentTick = Time.CalculateCurrentTick();

		while ( CurrentTick < expectedCurrentTick ) {
			CurrentTick++;
			_game?.Tick();

			PreviousHeldKeys = new( HeldKeys );
		}
	}

	static void onKeyDown( Key key ) => HeldKeys.Add( key );
	static void onKeyUp( Key key ) => HeldKeys.Remove( key );

	static Vector2 _lastMousePosition = default;
	static void onMouseMoved( Vector2 pos ) {
		if ( _lastMousePosition == default )
			_lastMousePosition = pos;

		var delta = _lastMousePosition - pos;
		_lastMousePosition = pos;

		if ( Graphics.MouseMode == MouseMode.Normal ) return;

		var lookDelta = new Angles( delta.X, delta.Y, 0f );
		LookDelta = lookDelta.Wrapped;
	}

	static void render() {
		_game?.Frame();

		LookDelta = Angles.Zero;
	}

	static void shutdown() {
		_game?.Shutdown();

		Graphics.Dispose();
	}
}
