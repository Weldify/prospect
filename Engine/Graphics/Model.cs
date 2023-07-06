using Prospect.Engine.OpenGL;

namespace Prospect.Engine;

public sealed class Model : IPreloadable {
	internal readonly static Dictionary<string, Model> Cache = new();

	internal IModel BackendModel { get; private set; } = null!;
	internal ITexture BackendTexture { get; private set; } = null!;

	bool IPreloadable.IsLoaded => _isLoaded;

	ModelResource _preset;
	bool _isLoaded = false;

	// Mark the constructor as private, prevent instantiation!
	Model( ModelResource preset ) {
		_preset = preset;
	}

	public static Model Load( string path ) {
		if ( Cache.TryGetValue( path, out var mdl ) )
			return mdl;

		if ( Resources.Get<ModelResource>( path ) is not ModelResource res )
			// TODO: Make an in-engine error model and return that instead of throwing
			throw new Exception( "Model ain't there pal" );

		Model model = new( res );

		if ( Entry.Graphics.HasLoaded )
			model.UpdateFromResource( res );

		Cache[path] = model;
		return model;
	}

	internal void UpdateFromResource( ModelResource res ) {
		var texture = Entry.Graphics.LoadTexture( res.TexturePath );
		var backendMesh = Entry.Graphics.LoadModel( res.MeshPath, texture );

		BackendModel = backendMesh;
		BackendTexture = texture;
	}

	void IPreloadable.Load() {
		if ( _isLoaded ) return;
		_isLoaded = true;

		Console.WriteLine( "amaload" );

		UpdateFromResource( _preset );
	}
}
