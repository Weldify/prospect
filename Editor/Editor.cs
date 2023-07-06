global using System;
global using System.Linq;
global using System.Collections.Generic;

using Prospect.Engine;
using System.IO;

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
		Window.OnFileDrop = onFileDrop;

		_settings = Resources.GetOrCreate<EditorSettings>( "settings.eds" );
		_projectManager.OpenProject( Path.Combine( _settings.LastProjectPath, "project.proj" ) );
	}

	public void Tick() { }

	public void Frame() {
		_projectManager.Draw();
	}

	public void Shutdown() {
		_settings.LastProjectPath = _projectManager.ProjectPath;
		_settings.Write( "settings.eds" );
	}

	void onFileDrop( string[] paths ) {
		if ( _projectManager.HasProject ) return;
		Console.WriteLine( "noitdon" );

		foreach ( var path in paths ) {
			_projectManager.OpenProject( path );
			_projectManager.CreateProject( path );
		}
	}
}
