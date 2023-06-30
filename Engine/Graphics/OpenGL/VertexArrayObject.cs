using Silk.NET.OpenGL;

namespace Prospect.Engine.OpenGL;

sealed class VertexArrayObject<VertexType, IndexType> : IDisposable
	where VertexType : unmanaged
	where IndexType : unmanaged {

	readonly uint _handle;
	readonly GL _gl;

	public VertexArrayObject( GL gl, BufferObject<VertexType> vbo, BufferObject<IndexType> ebo ) {
		_gl = gl;
		_handle = _gl.GenVertexArray();

		Bind();

		vbo.Bind();
		ebo.Bind();
	}

	public void SetVertexAttributePointer(uint index, int count, VertexAttribPointerType type, uint vertexSize, int offset) {
		unsafe {
			_gl.VertexAttribPointer( index, count, type, false, vertexSize * (uint)sizeof( VertexType ), (void*)(offset * sizeof( VertexType )) );
		}

		_gl.EnableVertexAttribArray( index );
	}

	public void Bind() {
		_gl.BindVertexArray( _handle );
	}

	public void Dispose() {
		_gl.DeleteVertexArray( _handle );
	}
}
