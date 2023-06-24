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
	ProjectManager _projectManager = new();

	static void Main() => Entry.Run<Editor>();

	public void Start() {
		Window.Title = "Prospect Editor";

		var settings = Resources.GetOrCreate<EditorSettings>( "settings.eds" );
		_projectManager.TryRestoreProject( settings.LastProjectPath );
	}

	public void Tick() {

	}

	public void Draw() {
		_projectManager.Draw();
	}

	public void Shutdown() {
		var settings = Resources.GetOrCreate<EditorSettings>( "settings.eds" );
		settings.LastProjectPath = _projectManager.ProjectPath;

		File.WriteAllText( "settings.eds", settings.Serialize() );
	}
}
