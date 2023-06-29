using Prospect.Engine.OpenGL;

namespace Prospect.Engine;

public sealed class Model : IPreloadable {
	internal readonly static Dictionary<string, Model> Cache = new();

	internal IModel BackendModel { get; private set; } = null!;
	internal ITexture BackendTexture { get; private set; } = null!;

	// When we replace unready models, we use this
	ModelResource _preset = null!;
	bool _isReady = false;

	// Mark the constructor as private, prevent instantiation!
	Model() { }

	public static Model Load( string path ) {
		if ( Cache.TryGetValue( path, out var mdl ) )
			return mdl;

		if ( Resources.Get<ModelResource>( path ) is not ModelResource res )
			throw new Exception( "Model ain't there pal" );

		Model model = new() {
			_preset = res,
			_isReady = true,
		};

		if ( !Entry.Graphics.IsReady )
			model._isReady = false;
		else
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

	void IPreloadable.Ready() {
		if ( _isReady ) return;
		_isReady = true;

		UpdateFromResource( _preset );
	}
}
