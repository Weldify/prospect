using Silk.NET.OpenGL;
using StbImageSharp;
using System;
using System.IO;

namespace Prospect.Engine.OpenGL;

public sealed class Texture : ITexture, IDisposable {
	public static Result<Texture> Load( GL gl, string path ) {
		if ( !File.Exists( path ) )
			return Result.Fail();

		// Load the image from memory.
		ImageResult result = ImageResult.FromMemory( File.ReadAllBytes( path ), ColorComponents.RedGreenBlueAlpha );
		
		return new Texture( gl, result );
	}

	readonly uint _handle;
	readonly GL _gl;

	unsafe Texture( GL gl, ImageResult imageResult ) {
		_gl = gl;
		_handle = _gl.GenTexture();
		Bind();

		fixed ( byte* ptr = imageResult.Data ) {
			// Create our texture and upload the image data.
			_gl.TexImage2D( TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)imageResult.Width,
				(uint)imageResult.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr );
		}

		setParameters();
	}

	void setParameters() {
		//Setting some texture perameters so the texture behaves as expected.
		_gl.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge );
		_gl.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge );
		_gl.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.LinearMipmapLinear );
		_gl.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear );
		_gl.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0 );
		_gl.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8 );
        
		//Generating mipmaps.
		_gl.GenerateMipmap( TextureTarget.Texture2D );
	}

	public void Bind( TextureUnit textureSlot = TextureUnit.Texture0 ) {
		//When we bind a texture we can choose which textureslot we can bind it to.
		_gl.ActiveTexture( textureSlot );
		_gl.BindTexture( TextureTarget.Texture2D, _handle );
	}

	public void Dispose() {
		//In order to dispose we need to delete the opengl handle for the texure.
		_gl.DeleteTexture( _handle );
	}
}
