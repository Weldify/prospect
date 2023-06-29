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

	PolygonMode _polygonMode;
	readonly Window _window;

	GL _gl = null!;
	Mesh arcMesh;
	Shader _shader = null!;
	Texture _texture = null!;
	Transform _transform = Transform.Zero;

	public GraphicsBackend() {
		_window = new();
		_window.Load += onLoad;
		_window.DoRender += doRender;
	}

	public void RunLoop() {
		_window.Run();
	}

	public void Dispose() {
		_window.Dispose();
	}

	void onLoad() {
		_gl = _window.Gl;

		// 3 vert, 2 tex coord
		float[] vertices = {
			-0.5f,  0.5f,  0.0f,  0.0f, 0.0f,
			-0.5f, -0.5f,  0.0f,  0.0f, 1.0f,
			 0.0f, -0.5f,  0.0f,  0.5f, 1.0f,
			 0.0f,  0.5f,  0.0f,  0.5f, 0.0f,
			 0.5f,  0.5f,  0.0f,  1.0f, 0.0f,
		};

		uint[] indices = {
			3u, 0u, 1u,
			1u, 2u, 3u,
			3u, 2u, 4u
		};

		arcMesh = new( _gl, vertices, indices );

		_shader = new Shader( _gl, Shaders.VERTEX_SOURCE, Shaders.FRAGMENT_SOURCE );
		_texture = new Texture( _gl, "arcicon.png" );

		// Better blending of texture edges or some shit
		_gl.Enable( EnableCap.Blend );
		_gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
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

	static float _speen = 0f;

	public void DrawThingamabob() {
		_speen += 0.08f;
		_transform.Rotation = Rotation.FromYawPitchRoll( _speen, 0f, 0f );

		arcMesh.Bind();
		_texture.Bind( TextureUnit.Texture0 );

		_shader.Use();
		_shader.SetUniform( "uTexture", 0 );
		_shader.SetUniform( "uModel", _transform.ViewMatrix );
		_shader.SetUniform( "uView", _currentView );
		_shader.SetUniform( "uProjection", _currentProjection );

		unsafe {
			_gl.DrawElements( PrimitiveType.Triangles, 15, DrawElementsType.UnsignedInt, (void*)0 );
		}
	}
}
