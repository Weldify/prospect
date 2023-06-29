namespace Prospect.Engine;

public sealed class Model {
	readonly static Dictionary<string, Model> _cache = new();

	internal readonly IModel BackendModel;
	internal readonly ITexture BackendTexture;

	Model( IModel model, ITexture texture ) {
		BackendModel = model;
		BackendTexture = texture;
	}

	public static Model Load( string path ) {
		if ( _cache.TryGetValue( path, out var mdl ) )
			return mdl;

		if ( Resources.Get<ModelResource>( path ) is not ModelResource res )
			throw new Exception( "Model ain't there pal" );

		var texture = Entry.Graphics.LoadTexture( res.TexturePath );
		var backendMesh = Entry.Graphics.LoadModel( res.MeshPath, texture );

		Model model = new( backendMesh, texture );
		_cache[path] = model;

		return model;
	}
}
