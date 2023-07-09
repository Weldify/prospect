using Silk.NET.OpenGL;
using StbImageSharp;
using System;
using System.IO;

namespace Prospect.Engine.OpenGL;

public sealed class Texture : ITexture, IDisposable
{
    public static Result<Texture> Load( GL gl, string path, TextureFilter filter )
    {
        if ( !File.Exists( path ) )
            return Result.Fail();

        // Load the image from memory.
        var result = ImageResult.FromMemory( File.ReadAllBytes( path ), ColorComponents.RedGreenBlueAlpha );

        return new Texture( gl, result, filter );
    }

    readonly uint _handle;
    readonly GL _gl;

    unsafe Texture( GL gl, ImageResult imageResult, TextureFilter filter )
    {
        _gl = gl;
        _handle = _gl.GenTexture();
        Bind();

        fixed ( byte* ptr = imageResult.Data )
        {
            // Create our texture and upload the image data.
            _gl.TexImage2D( TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)imageResult.Width,
                (uint)imageResult.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr );
        }

        setParameters( filter );
    }

    void setParameters( TextureFilter filter )
    {
        var minFilter = filter switch
        {
            TextureFilter.Nearest => GLEnum.NearestMipmapLinear,
            TextureFilter.Linear or _ => GLEnum.LinearMipmapLinear,
        };

        var magFilter = filter switch
        {
            TextureFilter.Nearest => GLEnum.Nearest,
            TextureFilter.Linear or _ => GLEnum.Linear,
        };

        //Setting some texture perameters so the texture behaves as expected.
        _gl.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge );
        _gl.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge );
        _gl.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)minFilter );
        _gl.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)magFilter );
        _gl.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0 );
        _gl.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8 );

        //Generating mipmaps.
        _gl.GenerateMipmap( TextureTarget.Texture2D );
    }

    public void Bind( TextureUnit textureSlot = TextureUnit.Texture0 )
    {
        //When we bind a texture we can choose which textureslot we can bind it to.
        _gl.ActiveTexture( textureSlot );
        _gl.BindTexture( TextureTarget.Texture2D, _handle );
    }

    public void Dispose()
    {
        //In order to dispose we need to delete the opengl handle for the texure.
        _gl.DeleteTexture( _handle );
    }
}
