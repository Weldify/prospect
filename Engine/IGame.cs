namespace Prospect.Engine;

public interface IGame {
	public void Start() { }
	public void Shutdown() { }

	public void Tick() { }
	public void Draw() { }
}
