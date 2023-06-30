namespace Prospect.Editor;

partial class ProjectManager {
	const string _SAMPLE_GAME_CODE =
@"using Prospect.Engine;

namespace YourGame;

public class Game : IGame {
	public GameOptions Options => GameOptions.Default;

	public void Start() { }
	public void Shutdown() { }

	public void Tick() { }
	public void Frame() { }
}";
}
