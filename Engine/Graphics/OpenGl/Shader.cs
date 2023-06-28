using Silk.NET.OpenGL;
using System.IO;
using System.Numerics;

namespace Prospect.Engine.OpenGl;

sealed class Shader : IDisposable {
	//Our handle and the GL instance this class will use, these are private because they have no reason to be public.
	//Most of the time you would want to abstract items to make things like this invisible.
	readonly uint _handle;
	readonly GL _gl;

	public Shader( GL gl, string vertexSource, string fragmentSource ) {
		_gl = gl;

		//Load the individual shaders.
		uint vertex = loadShader( ShaderType.VertexShader, vertexSource );
		uint fragment = loadShader( ShaderType.FragmentShader, fragmentSource );
		//Create the shader program.
		_handle = _gl.CreateProgram();
		//Attach the individual shaders.
		_gl.AttachShader( _handle, vertex );
		_gl.AttachShader( _handle, fragment );
		_gl.LinkProgram( _handle );
		//Check for linking errors.
		_gl.GetProgram( _handle, GLEnum.LinkStatus, out var status );
		if ( status == 0 ) {
			throw new Exception( $"Program failed to link with error: {_gl.GetProgramInfoLog( _handle )}" );
		}
		//Detach and delete the shaders
		_gl.DetachShader( _handle, vertex );
		_gl.DetachShader( _handle, fragment );
		_gl.DeleteShader( vertex );
		_gl.DeleteShader( fragment );
	}

	public void Use() {
		//Using the program
		_gl.UseProgram( _handle );
	}

	int getUniformLocation( string name ) {
		var location = _gl.GetUniformLocation( _handle, name );
		return location switch {
			-1 => throw new Exception( $"{name} uniform not found on shader." ),
			_ => location
		};
	}

	//Uniforms are properties that applies to the entire geometry
	public void SetUniform( string name, int value ) => _gl.Uniform1( getUniformLocation( name ), value );
	public void SetUniform( string name, float value ) => _gl.Uniform1( getUniformLocation( name ), value );
	public unsafe void SetUniform( string name, Matrix4x4 value ) =>
		_gl.UniformMatrix4( getUniformLocation( name ), 1, false, (float*)&value );

	public void Dispose() {
		//Remember to delete the program when we are done.
		_gl.DeleteProgram( _handle );
	}

	uint loadShader( ShaderType type, string src ) {
		uint handle = _gl.CreateShader( type );
		_gl.ShaderSource( handle, src );
		_gl.CompileShader( handle );
		string infoLog = _gl.GetShaderInfoLog( handle );
		if ( !string.IsNullOrWhiteSpace( infoLog ) ) {
			throw new Exception( $"Error compiling shader of type {type}, failed with error {infoLog}" );
		}

		return handle;
	}
}
