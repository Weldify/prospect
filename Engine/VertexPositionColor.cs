using System.Numerics;
using Veldrid;

namespace Prospect.Engine;

struct VertexPositionColor {
	// TODO: I don't like this. Will have to find a way to automatically determine this later
	public const uint SIZE_IN_BYTES = 24;

	public Vector2 Position;
	public RgbaFloat Color;

	public VertexPositionColor( Vector2 position, RgbaFloat color ) {
		Position = position;
		Color = color;
	}
}