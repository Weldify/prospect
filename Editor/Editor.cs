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

	public void Start() {
		Window.Title = "Prospect Editor";

		_settings = Resources.GetOrCreate<EditorSettings>( "settings.eds" );
		_projectManager.TryRestoreProject( _settings.LastProjectPath );
	}

	public void Tick() {
		TimeSince poop = 0f;

		if ( poop > 1f ) {

		}
	}

	//readonly Model _prospectIcon = Model.Load( "C:/Users/ian/Documents/Models/sword/longsword.mdl" );
	float _speen = 0f;

	public void Render() {
		_projectManager.Draw();
		_speen += 0.04f;

		//Camera.Transform = new Transform( -Vector3f.Forward, Rotation.LookAt( -Vector3f.Forward, Vector3f.Zero ) );
		//Graphics.DrawModel( _prospectIcon, new Transform( Vector3f.Zero, Rotation.FromYawPitchRoll( _speen, 90f, 0f ), 1f ) );
	}

	public void Shutdown() {
		_settings.LastProjectPath = _projectManager.ProjectPath;
		_settings.Write( "settings.eds" );
	}
}
