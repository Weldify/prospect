﻿using Silk.NET.Input;

namespace Prospect.Engine.OpenGL;

partial class GraphicsBackend {
	public MouseMode MouseMode {
		get => _mouseMode;
		set {
			_mouseMode = value;
			updateCursorMode();
		}
	}

	public Action<Key> KeyDown { get; set; } = ( k ) => { };
	public Action<Key> KeyUp { get; set; } = ( k ) => { };
	public Action<Vector2> MouseMoved { get; set; } = ( v ) => { };

	MouseMode _mouseMode;

	void initInput() {
		_inputContext = _window.CreateInput();

		if ( _inputContext.Keyboards.FirstOrDefault() is IKeyboard kb ) {
			kb.KeyDown += onKeyDown;
			kb.KeyUp += onKeyUp;
		}

		for ( int i = 0; i < _inputContext.Mice.Count; i++ ) {
			_inputContext.Mice[i].MouseMove += onMouseMove;
		}
	}

	void onKeyDown( IKeyboard kb, Silk.NET.Input.Key key, int i ) {
		KeyDown.Invoke( (Key)key );
	}

	void onKeyUp( IKeyboard kb, Silk.NET.Input.Key key, int i ) {
		KeyUp.Invoke( (Key)key );
	}

	void onMouseMove( IMouse mouse, System.Numerics.Vector2 pos ) {
		MouseMoved.Invoke( new Vector2( pos.X, pos.Y ) );
	}

	void updateCursorMode() {
		if ( _inputContext is null ) return;
		for ( int i = 0; i < _inputContext.Mice.Count; i++ ) {
			_inputContext.Mice[i].Cursor.CursorMode = (CursorMode)_mouseMode;
		}
	}
}
