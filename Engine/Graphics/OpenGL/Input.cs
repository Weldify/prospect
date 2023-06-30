namespace Prospect.Engine.OpenGL;

class Input : IInput {
	public Action<Key> KeyDown { get; set; } = ( k ) => { };
	public Action<Key> KeyUp { get; set; } = ( k ) => { };
}
