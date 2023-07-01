global using System;
global using System.Linq;
global using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using ImGuiNET;

namespace Prospect.Engine;

public static partial class Entry {
	// Tick
	internal static uint CurrentTick { get; private set; } = 0;
	public static uint TickRate { get; private set; } = 0;
	public static float TickDelta { get; private set; } = 0f;

	// Frame
	public static float FrameDelta { get; private set; } = 0f;

	internal static Stopwatch RawGameTime { get; private set; } = new();

	// Input
	internal static Angles LookDelta { get; private set; } = Angles.Zero;
	internal static HashSet<Key> HeldKeys { get; private set; } = new();
	internal static HashSet<Key> PreviousHeldKeys { get; private set; } = new();
	internal static HashSet<MouseButton> HeldButtons { get; private set; } = new();
	internal static HashSet<MouseButton> PreviousHeldButtons { get; private set; } = new();

	internal static IGraphicsBackend Graphics { get; private set; }

	static IGame? _game;
	static bool _hasGameStarted = false;

	static Entry() {
		Graphics = new OpenGL.GraphicsBackend {
			OnLoad = onGraphicsLoaded,
			OnUpdate = onUpdate,
			OnRender = onRender,
			KeyDown = onKeyDown,
			KeyUp = onKeyUp,
			MouseMoved = onMouseMoved,
			MouseDown = onMouseDown,
			MouseUp = onMouseUp
		};
	}

	static void applyOptions( IGame game ) {
		TickRate = game.Options.TickRate;
		TickDelta = 1f / (float)TickRate;
	}

	static void tryStartGame() {
		if ( _game is null || !Graphics.HasLoaded || _hasGameStarted ) return;
		_hasGameStarted = true;
		_game.Start();
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

	static void shutdown() {
		_game?.Shutdown();
	}

	static void onGraphicsLoaded() {
		// Load preloadable models
		Model.Cache.Values
			.Cast<IPreloadable>()
			.Where( m => !m.IsLoaded )
			.ToList()
			.ForEach( m => m.Load() );

		tryStartGame();
	}

	static void onUpdate() {
		var expectedCurrentTick = Time.CalculateCurrentTick();

		while ( CurrentTick < expectedCurrentTick ) {
			CurrentTick++;
			_game?.Tick();
		}
	}

	static void onRender( float delta ) {
		FrameDelta = delta;

		_game?.Frame();

		PreviousHeldKeys = new( HeldKeys );
		PreviousHeldButtons = new( HeldButtons );
		LookDelta = Angles.Zero;
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

	static void onMouseDown( MouseButton button ) => HeldButtons.Add( button );
	static void onMouseUp( MouseButton button ) => HeldButtons.Remove( button );
}
