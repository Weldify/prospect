using Silk.NET.OpenGL;
using StbImageSharp;
using System;
using System.Drawing;
using System.IO;
using System.Numerics;

namespace Prospect.Engine.OpenGl;

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

	public Action OnRender { private get; set; } = () => { };
	public Action OnLoad { private get; set; } = () => { };

	public bool IsReady { get; private set; } = false;

	PolygonMode _polygonMode;
	readonly Window _window;

	GL _gl = null!;
	Shader _shader = null!;

	public GraphicsBackend() {
		_window = new();
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
		_shader.SetUniform( "uView", _currentView );
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
		_gl = _window.Gl;
		_shader = new Shader( _gl, Shaders.VERTEX_SOURCE, Shaders.FRAGMENT_SOURCE );

		// Better blending of texture edges or some shit
		_gl.Enable( EnableCap.Blend );
		_gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );

		IsReady = true;

		OnLoad.Invoke();
	}

	Matrix4x4 _currentProjection;
	Matrix4x4 _currentView;

	void doRender() {
		_currentProjection = Matrix4x4.CreatePerspectiveFieldOfView( Camera.FieldOfView.ToRadians(), _window.Size.Aspect, 0.1f, 100f );
		_currentView = Camera.Transform.ViewMatrix;

		_gl.ClearColor( Color.FromArgb( 255, 0, 0, 0 ) );
		_gl.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

		OnRender.Invoke();
	}
}
