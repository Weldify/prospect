namespace Prospect.Engine;

public interface IGame {
	void Start() { }
	void Shutdown() { }

	void Tick() { }
	void Draw() { }
}