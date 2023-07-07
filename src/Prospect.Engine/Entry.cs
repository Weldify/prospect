using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
	internal static float ScrollDelta { get; private set; } = 0f;
	internal static HashSet<Key> HeldKeys { get; private set; } = new();
	internal static HashSet<Key> PreviousHeldKeys { get; private set; } = new();
	internal static HashSet<MouseButton> HeldButtons { get; private set; } = new();
	internal static HashSet<MouseButton> PreviousHeldButtons { get; private set; } = new();

	internal static IGraphicsBackend Graphics { get; private set; }
    internal static IAudioBackend Audio { get; private set; }

	static IGame? _game;
	static bool _hasGameStarted = false;

	static Entry() {
		Graphics = new OpenGL.GraphicsBackend {
			OnLoad = onGraphicsLoaded,
			OnUpdate = onUpdate,
			OnRender = onRender,
			OnFileDrop = onFileDrop,
			KeyDown = onKeyDown,
			KeyUp = onKeyUp,
			MouseMoved = onMouseMoved,
			MouseDown = onMouseDown,
			MouseUp = onMouseUp,
			Scroll = onScroll
		};
	}

	static void applyOptions( IGame game ) {
		TickRate = game.Options.TickRate;
		TickDelta = 1f / (float)TickRate;
	}

	static Status startGame() {
		if ( _game is null || !Graphics.HasLoaded || _hasGameStarted ) 
            return Status.Fail();

		_hasGameStarted = true;
		_game.Start();

        return Status.Ok();
	}

    public static void Run<T>() where T : IGame, new()
    {
        var game = new T();
        applyOptions( game );

        RawGameTime.Start();

        _game = game;
        _ = startGame();

        Graphics.RunLoop();

        shutdown();
    }

	static void shutdown() => _game?.Shutdown();

	static void onGraphicsLoaded() {
        // Load preloadable models
        foreach ( var model in Model._cache.Values.Where( m => !m._hasLoaded ) )
            model.postBackendLoad();

		_ = startGame();
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
		ScrollDelta = 0f;
	}

	static void onFileDrop( string[] paths ) => Window.OnFileDrop.Invoke( paths );

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
	static void onScroll( float side ) => ScrollDelta = side;
}
