namespace Prospect.Editor;

partial class Editor {
	const string SAMPLE_GAME_CODE = @"
using Prospect.Engine;

namespace YourGame;

public class Game : IGame {
	void Start() { }
	void Shutdown() { }

	void Tick() { }
	void Draw() { }
}
";
}
