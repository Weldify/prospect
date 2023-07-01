namespace Prospect.Engine;

/// <summary> This asset relies on the graphics backend and might not be fully loaded right away </summary>
interface IPreloadable {
	bool IsLoaded { get; }
	/// <summary> Runs when the graphics backend has loaded </summary>
	void Load();
}
