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

	}

	public void Draw() {
		_projectManager.Draw();
	}

	public void Shutdown() {
		_settings.LastProjectPath = _projectManager.ProjectPath;
		_settings.Write( "settings.eds" );
	}
}
