using System.Diagnostics.CodeAnalysis;

namespace Prospect.Engine;

public struct TimeSince : IEquatable<TimeSince> {
	public float StartTime;

	public TimeSince( float startTime ) => StartTime = startTime;

	public static bool operator ==( TimeSince t1, TimeSince t2 ) => t1.StartTime == t2.StartTime;
	public static bool operator !=( TimeSince t1, TimeSince t2 ) => t1.StartTime != t2.StartTime;

	public bool Equals( TimeSince other ) => this == other;
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is TimeSince other && this == other;
	public override int GetHashCode() => HashCode.Combine( StartTime );

	public static implicit operator TimeSince( float time ) => new() { StartTime = time };
	public static implicit operator float( TimeSince t ) => Time.Now - t.StartTime;
}
