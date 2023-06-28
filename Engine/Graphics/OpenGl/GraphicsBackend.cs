using Silk.NET.OpenGL;
using StbImageSharp;
using System;
using System.IO;

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

	GL _gl;
	uint _vao;
	uint _vbo;
	uint _ebo;
	uint _program;
	uint _texture;

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

		_vao = _gl.GenVertexArray();
		_gl.BindVertexArray( _vao );

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

		_vbo = _gl.GenBuffer();
		_gl.BindBuffer( BufferTargetARB.ArrayBuffer, _vbo );

		unsafe {
			fixed ( float* buffer = vertices )
				_gl.BufferData( BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof( float )), buffer, BufferUsageARB.StaticDraw );
		}

		_ebo = _gl.GenBuffer();
		_gl.BindBuffer( BufferTargetARB.ElementArrayBuffer, _ebo );

		unsafe {
			fixed ( uint* buffer = indices )
				_gl.BufferData(
					BufferTargetARB.ElementArrayBuffer,
					(nuint)(indices.Length * sizeof( uint )),
					buffer,
					BufferUsageARB.StaticDraw
				);
		}

		uint vertexShader = _gl.CreateShader( GLEnum.VertexShader );
		_gl.ShaderSource( vertexShader, Shaders.VERTEX_SOURCE );

		uint fragmentShader = _gl.CreateShader( GLEnum.FragmentShader );
		_gl.ShaderSource( fragmentShader, Shaders.FRAGMENT_SOURCE );

		_gl.CompileShader( vertexShader );
		_gl.GetShader( vertexShader, ShaderParameterName.CompileStatus, out int vStatus );
		if ( vStatus != (int)GLEnum.True )
			throw new Exception( $"Vertex shader failed to compile: {_gl.GetShaderInfoLog( vertexShader )}" );

		_gl.CompileShader( fragmentShader );
		_gl.GetShader( fragmentShader, ShaderParameterName.CompileStatus, out int fStatus );
		if ( fStatus != (int)GLEnum.True )
			throw new Exception( $"Fragment shader failed to compile: {_gl.GetShaderInfoLog( fragmentShader )}" );

		_program = _gl.CreateProgram();
		_gl.AttachShader( _program, vertexShader );
		_gl.AttachShader( _program, fragmentShader );

		_gl.LinkProgram( _program );

		_gl.GetProgram( _program, ProgramPropertyARB.LinkStatus, out int lStatus );
		if ( lStatus != (int)GLEnum.True )
			throw new Exception( $"Program failed to link: {_gl.GetProgramInfoLog( _program )}" );

		// The program was linked, everything needed to run it is now contained within it
		// Clear the leftover shiz

		_gl.DetachShader( _program, vertexShader );
		_gl.DetachShader( _program, fragmentShader );

		_gl.DeleteShader( vertexShader );
		_gl.DeleteShader( fragmentShader );

		const uint STRIDE = 3 * sizeof( float ) + 2 * sizeof( float );
		const uint POSITION_LOC = 0;
		const uint TEXTURE_LOC = 1;

		_gl.EnableVertexAttribArray( POSITION_LOC );
		unsafe {
			// Last argument is the offset. We only have 1 attribute - so its 0
			_gl.VertexAttribPointer( POSITION_LOC, 3, VertexAttribPointerType.Float, false, STRIDE, (void*)0 );
		}

		_gl.EnableVertexAttribArray( TEXTURE_LOC );
		unsafe {
			_gl.VertexAttribPointer( TEXTURE_LOC, 2, VertexAttribPointerType.Float, false, STRIDE, (void*)(3 * sizeof( float )) );
		}

		// "Unbind" our previous shit. Doesnt actually do anything
		_gl.BindVertexArray( 0 );
		_gl.BindBuffer( BufferTargetARB.ArrayBuffer, 0 );
		_gl.BindBuffer( BufferTargetARB.ElementArrayBuffer, 0 );

		_texture = _gl.GenTexture();
		_gl.ActiveTexture( TextureUnit.Texture0 );
		_gl.BindTexture( TextureTarget.Texture2D, _texture );

		var result = ImageResult.FromMemory( File.ReadAllBytes( "arcicon.png" ), ColorComponents.RedGreenBlueAlpha );

		unsafe {
			fixed ( byte* ptr = result.Data ) {
				_gl.TexImage2D(
					TextureTarget.Texture2D,
					0,
					InternalFormat.Rgba,
					(uint)result.Width,
					(uint)result.Height,
					0,
					PixelFormat.Rgba,
					PixelType.UnsignedByte,
					ptr
				);
			}
		}

		_gl.TextureParameter( _texture, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge );
		_gl.TextureParameter( _texture, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge );

		_gl.TextureParameter( _texture, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear );
		_gl.TextureParameter( _texture, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear );

		_gl.GenerateMipmap( TextureTarget.Texture2D );

		_gl.BindTexture( TextureTarget.Texture2D, 0 );

		var loc = _gl.GetUniformLocation( _program, "uTexture" );
		_gl.Uniform1( loc, 0 );

		_gl.Enable( EnableCap.Blend );
		_gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );
	}

	public void DrawThingamabob() {
		_gl.BindVertexArray( _vao );
		_gl.UseProgram( _program );

		_gl.ActiveTexture( TextureUnit.Texture0 );
		_gl.BindTexture( TextureTarget.Texture2D, _texture );

		unsafe {
			_gl.DrawElements( PrimitiveType.Triangles, 15, DrawElementsType.UnsignedInt, (void*)0 );
		}
	}
}
