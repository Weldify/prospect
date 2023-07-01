using System.Diagnostics.CodeAnalysis;

namespace Prospect.Engine;

public struct Point2 : IEquatable<Point2> {
	public int X;
	public int Y;

	public float Aspect => (float)X / (float)Y;

	public Point2( int x, int y ) {
		X = x;
		Y = y;
	}

	public static bool operator ==( Point2 a, Point2 b ) => a.X == b.X && a.Y == b.Y;
	public static bool operator !=( Point2 a, Point2 b ) => a.X != b.X || a.Y != b.Y;

	public bool Equals( Point2 other ) => this == other;
	public override bool Equals( [NotNullWhen( true )] object? obj ) => obj is Point2 other && this == other;
	public override int GetHashCode() => HashCode.Combine( X, Y );
}
