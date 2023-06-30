namespace Prospect.Engine;

public interface IGame {
	GameOptions Options { get; }

	void Start() { }
	void Shutdown() { }

	void Tick() { }
	void Frame() { }
}
