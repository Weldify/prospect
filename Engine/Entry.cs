global using System;
global using System.Linq;
global using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
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
	static bool _gameStarted = false;

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
		if ( !_gameStarted && _game is not null ) {
			_gameStarted = true;
			_game.Start();
		}

		var expectedCurrentTick = Time.CalculateCurrentTick();

		while ( CurrentTick < expectedCurrentTick ) {
			CurrentTick++;
			_game?.Tick();

			PreviousHeldKeys = new( HeldKeys );
			//HeldKeys.RemoveWhere( _liftedKeys.Contains );
			//_liftedKeys.Clear();
		}
	}

	static void onKeyDown( Key key ) {
		HeldKeys.Add( key );
	}

	static void onKeyUp( Key key ) {
		HeldKeys.Remove( key );
	}

	static Vector2f _lastMousePosition = new();

	static void onMouseMoved( Vector2f pos ) {
		var delta = pos - _lastMousePosition;
		_lastMousePosition = pos;

		LookDelta = (
			LookDelta with {
				Yaw = delta.X,
				Pitch = delta.Y,
				Roll = 0f
			}
		).Wrapped;
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
