using Silk.NET.OpenGL;

namespace Prospect.Engine.OpenGL;

sealed class BufferObject<DataType> : IDisposable where DataType : unmanaged {
	readonly uint _handle;
	readonly GL _gl;
	readonly BufferTargetARB _bufferType;

	public BufferObject( GL gl, BufferTargetARB bufferType, Span<DataType> data ) {
		_gl = gl;
		_bufferType = bufferType;

		_handle = gl.GenBuffer();
		Bind();

		unsafe {
			fixed ( void* d = data ) {
				_gl.BufferData( bufferType, (nuint)(data.Length * sizeof( DataType )), d, BufferUsageARB.StaticDraw );
			}
		}
	}

	public void Bind() {
		_gl.BindBuffer( _bufferType, _handle );
	}

	public void Dispose() {
		_gl.DeleteBuffer( _handle );
	}
}
