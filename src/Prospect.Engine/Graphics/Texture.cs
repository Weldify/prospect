using System;
using System.Collections.Generic;
using System.IO;

namespace Prospect.Engine;

public sealed class Texture
{
    internal readonly static Dictionary<string, Texture> _cache = new();

    public static Texture Load(string path)
    {
        if ( _cache.TryGetValue( path, out var tex ) )
            return tex;

        if ( Resources.Get<TextureResource>( path ) is not TextureResource res )
            // TODO: Make an in-engine error texture and return that instead of throwing
            throw new Exception( "Texture ain't there pal" );

        var texture = new Texture(res);

        if ( Entry.Graphics.HasLoaded )
        {
            texture.updateFromResource( res );
            texture._hasLoaded = true;
        }

        _cache[ path ] = texture;
        return texture;
    }

    internal ITexture BackendTexture { get; private set; } = null!;
    internal bool _hasLoaded = false;
    readonly TextureResource _preset;

    Texture( TextureResource preset ) => _preset = preset;

    void updateFromResource(TextureResource res)
    {
        var imagePath = Path.Combine( res.Directory, res.ImagePath );

        var texture = Entry.Graphics.LoadTexture( imagePath, res.Filter );

        BackendTexture = texture.Value;
    }

    internal void postBackendLoad()
    {
        if ( _hasLoaded ) return;
        _hasLoaded = true;

        updateFromResource( _preset );
    }
}