using Prospect.Engine.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;

namespace Prospect.Engine;

public sealed class Model
{
    internal readonly static Dictionary<string, Model> _cache = new();

    internal IModel BackendModel { get; private set; } = null!;
    internal Texture Texture { get; private set; }

    internal bool _hasLoaded = false;
    readonly ModelResource _preset;

    // Mark the constructor as private, prevent instantiation!
    Model( ModelResource preset, Texture texture )
    {
        _preset = preset;
        Texture = texture;
    }

    public static Model Load( string path )
    {
        if ( _cache.TryGetValue( path, out var mdl ) )
            return mdl;

        if ( Resources.Get<ModelResource>( path ) is not ModelResource res )
            // TODO: Make an in-engine error model and return that instead of throwing
            throw new Exception( "Model ain't there pal" );

        var texturePath = Path.Combine( res.Directory, res.TexturePath );
        var texture = Texture.Load( texturePath );

        Model model = new( res, texture );

        if ( Entry.Graphics.HasLoaded )
        {
            model.updateFromResource( res );
            model._hasLoaded = true;
        }

        _cache[ path ] = model;
        return model;
    }

    void updateFromResource( ModelResource res )
    {
        // These are relative
        var meshPath = Path.Combine( res.Directory, res.MeshPath );

        // TODO: Use placeholder error resources if these fail
        var backendMesh = Entry.Graphics.LoadModel( meshPath );

        BackendModel = backendMesh.Value;
    }

    internal void postBackendLoad()
    {
        if ( _hasLoaded ) return;
        _hasLoaded = true;

        updateFromResource( _preset );
    }
}
