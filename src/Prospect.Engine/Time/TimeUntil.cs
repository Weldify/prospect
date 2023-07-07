using System;
using System.Diagnostics.CodeAnalysis;

namespace Prospect.Engine;

public struct TimeUntil : IEquatable<TimeUntil> {
	public float GoalTime;

	public static bool operator ==( TimeUntil t1, TimeUntil t2 ) => t1.GoalTime == t2.GoalTime;
	public static bool operator !=( TimeUntil t1, TimeUntil t2 ) => t1.GoalTime != t2.GoalTime;

	public bool Equals( TimeUntil other ) => this == other;
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is TimeUntil other && this == other;
	public override int GetHashCode() => HashCode.Combine( GoalTime );

	public static implicit operator float( TimeUntil t ) => t.GoalTime - Time.Now;
	public static implicit operator TimeUntil( float time ) => new() { GoalTime = Time.Now + time };
}
