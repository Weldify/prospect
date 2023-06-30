using Silk.NET.Input;
using Silk.NET.OpenGL;
using StbImageSharp;
using System;
using System.Drawing;
using System.IO;
using System.Numerics;

namespace Prospect.Engine.OpenGL;

class GraphicsBackend : IGraphicsBackend {
	public IWindow Window => _window;
	public IInput Input => _input;
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

	public Action OnRender { private get; set; } = () => { };
	public Action OnLoad { private get; set; } = () => { };

	public bool IsReady { get; private set; } = false;

	PolygonMode _polygonMode;
	readonly Window _window = new();
	readonly Input _input = new();

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
		_shader.SetUniform( "uTransform", transform.ViewMatrix );
		_shader.SetUniform( "uView", Camera.Transform.ViewMatrix );
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
		IInputContext input = _window.CreateInput();
		if ( input.Keyboards.FirstOrDefault() is IKeyboard kb ) {
			kb.KeyDown += onKeyDown;
			kb.KeyUp += onKeyUp;
		}

		_shader = new Shader( _gl, Shaders.VERTEX_SOURCE, Shaders.FRAGMENT_SOURCE );

		// Better blending of texture edges or some shit
		_gl.Enable( EnableCap.Blend );
		_gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );

		_gl.Enable( EnableCap.DepthTest );

		IsReady = true;

		OnLoad.Invoke();
	}

	void onKeyDown( IKeyboard kb, Silk.NET.Input.Key key, int i ) {
		_input.KeyDown.Invoke( (Key)key );
	}

	void onKeyUp( IKeyboard kb, Silk.NET.Input.Key key, int i ) {
		_input.KeyUp.Invoke( (Key)key );
	}

	Matrix4x4 _currentProjection;

	void doRender() {
		_currentProjection = Matrix4x4.CreatePerspectiveFieldOfView( Camera.FieldOfView.ToRadians(), _window.Size.Aspect, 0.1f, 100f );

		_gl.ClearColor( Color.FromArgb( 255, 0, 0, 0 ) );
		_gl.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

		OnRender.Invoke();
	}
}
