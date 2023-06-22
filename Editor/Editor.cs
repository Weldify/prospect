using System;
using Prospect.Engine;

namespace Prospect.Editor;

class Editor : IGame {
	static void Main() {
		Entry.Run<Editor>();
	}

	public void Start() {
		Console.WriteLine( "Editor started!" );
	}

	public void Shutdown() {

	}

	public void Tick() {

	}

	public void Draw() {

	}
}