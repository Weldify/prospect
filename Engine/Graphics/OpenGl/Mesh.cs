using Silk.NET.OpenGL;

namespace Prospect.Engine.OpenGl;

class Mesh : IDisposable {
	public readonly uint VertexCount;
	public readonly uint IndiceCount;

	readonly BufferObject<float> _vbo;
	readonly BufferObject<uint> _ebo;
	readonly VertexArrayObject<float, uint> _vao;
	readonly Texture _texture;

	public Mesh( GL gl, float[] vertices, uint[] indices, Texture texture ) {
		VertexCount = (uint)vertices.Length;
		IndiceCount = (uint)indices.Length;

		_texture = texture;

		_vbo = new( gl, BufferTargetARB.ArrayBuffer, vertices );
		_ebo = new( gl, BufferTargetARB.ElementArrayBuffer, indices );
		_vao = new( gl, _vbo, _ebo );

		_vao.SetVertexAttributePointer( 0, 3, VertexAttribPointerType.Float, 5, 0 ); // Vertices
		_vao.SetVertexAttributePointer( 1, 2, VertexAttribPointerType.Float, 5, 3 ); // UVs
	}

	public void Bind() {
		_vao.Bind();
		_texture.Bind( TextureUnit.Texture0 );
	}

	public void Dispose() {
		_vao.Dispose();
		_vbo.Dispose();
		_ebo.Dispose();
	}
}
