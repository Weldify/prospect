﻿using System;

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
	Action<float> Scroll { get; set; }

	// Loop
	Action OnLoad { set; }
	Action OnUpdate { set; }
	Action<float> OnRender { set; }
	Action<string[]> OnFileDrop { set; }
	void RunLoop();

	// Rendering
	Result<IModel> LoadModel( string path );
	Result<ITexture> LoadTexture( string path, TextureFilter filter );

	PolygonMode PolygonMode { get; set; }

	void DrawModel( Model model, Transform transform );
}
