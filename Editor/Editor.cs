global using System;
global using System.Linq;
global using System.Collections.Generic;

using Microsoft.CodeAnalysis.CSharp;
using System.Numerics;
using System.IO;

using ImGuiNET;

using Prospect.Engine;
using Microsoft.CodeAnalysis;

namespace Prospect.Editor;

partial class Editor : IGame {
	readonly ProjectManager _projectManager = new();

	EditorSettings _settings = new();

	static void Main() => Entry.Run<Editor>();

	public GameOptions Options => GameOptions.Default with {
		TickRate = 60
	};

	public void Start() {
		Window.Title = "Prospect Editor";

		_settings = Resources.GetOrCreate<EditorSettings>( "settings.eds" );
		_projectManager.TryRestoreProject( _settings.LastProjectPath );
	}

	public void Tick() {
		if ( Input.Pressed( Key.Space ) ) {
			Console.WriteLine( $"wepres {Time.Tick}" );
		}

		if ( Input.Down( Key.Space ) ) {
			Console.WriteLine( $"wehold {Time.Tick}" );
		}
	}

	readonly Model _prospectIcon = Model.Load( "C:/Users/ian/Documents/Models/sword/longsword.mdl" );

	public void Frame() {
		_projectManager.Draw();

		if ( !Input.Down( Key.Space ) ) return;

		var transform = new Transform( Vector3f.Zero, (Rotation)new Angles( Time.Now, 90f, 0f ) );

		Camera.Transform = new Transform( -Vector3f.Forward, Rotation.LookAt( -Vector3f.Forward, Vector3f.Zero ) );
		Graphics.DrawModel( _prospectIcon, transform );
	}

	public void Shutdown() {
		_settings.LastProjectPath = _projectManager.ProjectPath;
		_settings.Write( "settings.eds" );
	}
}
