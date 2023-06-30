namespace Prospect.Engine;

interface IInput {
	Action<Key> KeyDown { get; set; }
	Action<Key> KeyUp { get; set; }
}
