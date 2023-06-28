using Silk.NET.OpenGL;
using StbImageSharp;
using System;
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

	PolygonMode _polygonMode;
	readonly Window _window;

	GL _gl = null!;
	BufferObject<float> _vbo = null!;
	BufferObject<uint> _ebo = null!;
	VertexArrayObject<float, uint> _vao = null!;
	Shader _shader = null!;
	Texture _texture = null!;
	Transform _transform = Transform.Zero;

	Transform _camera = new( new( 0f, 0f, -1f ), Rotation.LookAt( new( 0f, 0f, -1f ), Vector3f.Zero ) );

	public GraphicsBackend() {
		_window = new();
		_window.Load += onLoad;
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

		_vbo = new( _gl, BufferTargetARB.ArrayBuffer, vertices );
		_ebo = new( _gl, BufferTargetARB.ElementArrayBuffer, indices );
		_vao = new( _gl, _vbo, _ebo );

		_vao.SetVertexAttributePointer( 0, 3, VertexAttribPointerType.Float, 5, 0 );
		_vao.SetVertexAttributePointer( 1, 2, VertexAttribPointerType.Float, 5, 3 );

		_shader = new Shader( _gl, Shaders.VERTEX_SOURCE, Shaders.FRAGMENT_SOURCE );
		_texture = new Texture( _gl, "arcicon.png" );

		// Better blending of texture edges or some shit
		_gl.Enable( EnableCap.Blend );
		_gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
	}

	static float _speen = 0f;

	public void DrawThingamabob() {
		_vao.Bind();
		_shader.Use();

		_texture.Bind( TextureUnit.Texture0 );

		_speen += 0.08f;
		_transform.Rotation = Rotation.FromYawPitchRoll( _speen, 0f, 0f );

		var projection = Matrix4x4.CreatePerspectiveFieldOfView( 70f.ToRadians(), _window.Size.Aspect, 0.1f, 100f );

		_shader.SetUniform( "uTexture", 0 );
		_shader.SetUniform( "uModel", _transform.ViewMatrix );
		_shader.SetUniform( "uView", _camera.ViewMatrix );
		_shader.SetUniform( "uProjection", projection );

		unsafe {
			_gl.DrawElements( PrimitiveType.Triangles, 15, DrawElementsType.UnsignedInt, (void*)0 );
		}
	}
}
