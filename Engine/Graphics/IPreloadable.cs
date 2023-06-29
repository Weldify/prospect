namespace Prospect.Engine;

/// <summary> This asset can be preloaded by the user, but won't actually be ready until some point in time </summary>
interface IPreloadable {
	/// <summary> Runs when this should be loaded </summary>
	internal void Ready();
}
