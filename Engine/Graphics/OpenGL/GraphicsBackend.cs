using Silk.NET.Input;
using Silk.NET.OpenGL;
using StbImageSharp;
using System;
using System.Drawing;
using System.IO;

using Matrix4x4 = System.Numerics.Matrix4x4;

namespace Prospect.Engine.OpenGL;

class GraphicsBackend : IGraphicsBackend {
	public IWindow Window => _window;
	public PolygonMode PolygonMode {
		get => _polygonMode;
		set {
			if ( value == _polygonMode ) return;
			_polygonMode = value;

			switch ( _polygonMode ) {
				case PolygonMode.Normal:
					_gl.PolygonMode( GLEnum.FrontAndBack, GLEnum.Fill );
					break;
				case PolygonMode.Wireframe:
					_gl.PolygonMode( GLEnum.FrontAndBack, GLEnum.Line );
					break;
			}
		}
	}

	public MouseMode MouseMode {
		get => _mouseMode;
		set {
			if ( value == _mouseMode ) return;
			_mouseMode = value;

			for ( int i = 0; i < _input.Mice.Count; i++ ) {
				_input.Mice[i].Cursor.CursorMode = (CursorMode)_mouseMode;
			}
		}
	}

	public Action OnRender { private get; set; } = () => { };
	public Action OnLoad { private get; set; } = () => { };
	public Action<Key> KeyDown { get; set; } = ( k ) => { };
	public Action<Key> KeyUp { get; set; } = ( k ) => { };
	public Action<Vector2> MouseMoved { get; set; } = ( v ) => { };

	public bool IsReady { get; private set; } = false;

	PolygonMode _polygonMode;
	MouseMode _mouseMode;
	readonly Window _window = new();
	IInputContext _input = null!;

	GL _gl = null!;
	Shader _shader = null!;

	public GraphicsBackend() {
		_window.Load += onLoad;
		_window.DoRender += doRender;
	}

	public IModel LoadModel( string path, ITexture texture ) {
		var tex = texture as Texture ?? throw new Exception( "Booboo exception" );
		return new Model( _gl, path, tex );
	}

	public ITexture LoadTexture( string path ) {
		return new Texture( _gl, path );
	}

	public void RunLoop() {
		_window.Run();
	}

	public void DrawModel( Engine.Model model, Transform transform ) {
		_shader.Use();
		_shader.SetUniform( "uTexture", 0 );
		_shader.SetUniform( "uTransform", transform.Matrix );
		_shader.SetUniform( "uView", Camera.ViewMatrix );
		_shader.SetUniform( "uProjection", _currentProjection );

		var backendModel = model.BackendModel as Model ?? throw new Exception( "Bad" );

		foreach ( var mesh in backendModel.Meshes ) {
			mesh.Bind();

			unsafe {
				_gl.DrawElements( PrimitiveType.Triangles, mesh.IndiceCount, DrawElementsType.UnsignedInt, null );
			}
		}
	}

	public void Dispose() {
		_window.Dispose();
	}

	void onLoad() {
		_gl = _window.GL;
		_input = _window.CreateInput();

		if ( _input.Keyboards.FirstOrDefault() is IKeyboard kb ) {
			kb.KeyDown += onKeyDown;
			kb.KeyUp += onKeyUp;
		}

		for ( int i = 0; i < _input.Mice.Count; i++ ) {
			_input.Mice[i].MouseMove += onMouseMove;
		}

		_shader = new Shader( _gl, Shaders.VERTEX_SOURCE, Shaders.FRAGMENT_SOURCE );

		// Better blending of texture edges or some shit
		_gl.Enable( EnableCap.Blend );
		_gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );

		_gl.Enable( EnableCap.DepthTest );
		_gl.Enable( EnableCap.CullFace );

		IsReady = true;

		OnLoad.Invoke();
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

	Matrix4x4 _currentProjection;

	void doRender() {
		_currentProjection = Matrix4x4.CreatePerspectiveFieldOfView( Camera.FieldOfView.ToRadians(), _window.Size.Aspect, 0.1f, 100f );

		_gl.ClearColor( Color.FromArgb( 255, 0, 0, 0 ) );
		_gl.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

		OnRender.Invoke();
	}
}
