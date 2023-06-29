namespace Prospect.Engine.OpenGl;

struct Vertex {
	public const uint MAX_BONE_INFLUENCE = 4;

	public Vector3f Position;
	public Vector3f Normal;
	public Vector3f Tangent;
	public Vector3f BiTangent;
	public Vector2f TexCoords;

	public int[] BoneIds;
	public float[] Weights;
}
