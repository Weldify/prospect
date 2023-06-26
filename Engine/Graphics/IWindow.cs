namespace Prospect.Engine;

interface IWindow {
	string Title { get; set; }

	Action<float>? DoUpdate { get; set; }
	Action? DoRender { get; set; }
}
