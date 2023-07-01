namespace Prospect.Engine;

interface IGraphicsBackend {
	// State
	/// <summary> Is the backend ready to be used?</summary>
	bool HasLoaded { get; }

	// Windowing
	/// <summary> If the backend has a window, use this as the title </summary>
	string WindowTitle { get; set; }

	// Input
	MouseMode MouseMode { get; set; }
	Action<Key> KeyDown { get; set; }
	Action<Key> KeyUp { get; set; }
	Action<Vector2> MouseMoved { get; set; }
	Action<MouseButton> MouseDown { get; set; }
	Action<MouseButton> MouseUp { get; set; }

	// Loop
	Action OnLoad { set; }
	Action OnUpdate { set; }
	Action<float> OnRender { set; }
	void RunLoop();

	// Rendering
	IModel LoadModel( string path, ITexture texture );
	ITexture LoadTexture( string path );

	PolygonMode PolygonMode { get; set; }

	void DrawModel( Model model, Transform transform );
}
