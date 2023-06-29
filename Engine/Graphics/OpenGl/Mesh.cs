using Silk.NET.OpenGL;

namespace Prospect.Engine.OpenGl;

class Mesh : IDisposable {
	readonly BufferObject<float> _vbo;
	readonly BufferObject<uint> _ebo;
	readonly VertexArrayObject<float, uint> _vao;

	public Mesh( GL gl, float[] vertices, uint[] indices ) {
		_vbo = new( gl, BufferTargetARB.ArrayBuffer, vertices );
		_ebo = new( gl, BufferTargetARB.ElementArrayBuffer, indices );
		_vao = new( gl, _vbo, _ebo );

		_vao.SetVertexAttributePointer( 0, 3, VertexAttribPointerType.Float, 5, 0 ); // Vertices
		_vao.SetVertexAttributePointer( 1, 2, VertexAttribPointerType.Float, 5, 3 ); // UVs
	}

	public void Bind() {
		_vao.Bind();
	}

	public void Dispose() {
		_vao.Dispose();
		_vbo.Dispose();
		_ebo.Dispose();
	}
}
