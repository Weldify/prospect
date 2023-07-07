﻿using Prospect.Engine.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;

namespace Prospect.Engine;

public sealed class Model {
	internal readonly static Dictionary<string, Model> _cache = new();

	internal IModel BackendModel { get; private set; } = null!;
	internal ITexture BackendTexture { get; private set; } = null!;

    internal bool _hasLoaded = false;
	readonly ModelResource _preset;

	// Mark the constructor as private, prevent instantiation!
	Model( ModelResource preset ) => _preset = preset;

	public static Model Load( string path ) {
		if ( _cache.TryGetValue( path, out var mdl ) )
			return mdl;

		if ( Resources.Get<ModelResource>( path ) is not ModelResource res )
			// TODO: Make an in-engine error model and return that instead of throwing
			throw new Exception( "Model ain't there pal" );

		Model model = new( res );

		if ( Entry.Graphics.HasLoaded )
			model.updateFromResource( res );

		_cache[path] = model;
		return model;
	}

	void updateFromResource( ModelResource res ) {
        // These are relative
        var texturePath = Path.Combine( res.Directory, res.TexturePath );
        var meshPath = Path.Combine( res.Directory, res.MeshPath );

        // TODO: Use placeholder error resources if these fail
        var texture = Entry.Graphics.LoadTexture( texturePath );
		var backendMesh = Entry.Graphics.LoadModel( meshPath, texture.Value );

		BackendModel = backendMesh.Value;
		BackendTexture = texture.Value;
	}

	internal void postBackendLoad() {
		if ( _hasLoaded ) return;
		_hasLoaded = true;

		Console.WriteLine( "amaload" );

        updateFromResource( _preset );
	}
}
