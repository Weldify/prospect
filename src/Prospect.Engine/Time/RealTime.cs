namespace Prospect.Engine;

public static class RealTime
{
    // Frames
    public static float Delta => Entry.FrameDelta;

    /// <summary> Raw time since startup </summary>
    public static float Now => (float) Entry.RawGameTime.Elapsed.TotalSeconds;
}