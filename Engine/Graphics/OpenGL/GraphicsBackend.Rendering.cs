using Silk.NET.OpenGL;
using System.Drawing;
using Matrix4x4 = System.Numerics.Matrix4x4;

namespace Prospect.Engine.OpenGL;

partial class GraphicsBackend {
	public PolygonMode PolygonMode {
		get => _polygonMode;
		set {
			_polygonMode = value;

			switch ( _polygonMode ) {
				case PolygonMode.Normal:
					_gl.PolygonMode( GLEnum.FrontAndBack, GLEnum.Fill );
					break;
				case PolygonMode.Wireframe:
					_gl.PolygonMode( GLEnum.FrontAndBack, GLEnum.Line );
					break;
			}
		}
	}

	public Action OnLoad { private get; set; } = () => { };
	public Action OnUpdate { private get; set; } = () => { };
	public Action<float> OnRender { private get; set; } = ( f ) => { };

	PolygonMode _polygonMode;

	void initRendering() {
		_mainModelShader = new Shader( _gl, Shaders.VERTEX_SOURCE, Shaders.FRAGMENT_SOURCE );

		// Better blending of texture edges or some shit
		_gl.Enable( EnableCap.Blend );
		_gl.BlendFunc( BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha );

		_gl.Enable( EnableCap.DepthTest );
		_gl.Enable( EnableCap.CullFace );
	}

	Matrix4x4 _currentProjection;
	void beginRender( float delta ) {
		// Calculate camera projection that will be used for this frame
		Point2 windowSize = new( _window.Size.X, _window.Size.Y );
		_currentProjection = Matrix4x4.CreatePerspectiveFieldOfView( Camera.FieldOfView.ToRadians(), windowSize.Aspect, 0.1f, 100f );

		_gl.ClearColor( Color.FromArgb( 255, 0, 0, 0 ) );
		_gl.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

		_imGuiController?.Update( delta );
	}

	void endRender() {
		_imGuiController?.Render();
	}

	public IModel LoadModel( string path, ITexture texture ) {
		var tex = texture as Texture ?? throw new Exception( "Booboo exception" );
		return new Model( _gl, path, tex );
	}

	public ITexture LoadTexture( string path ) {
		return new Texture( _gl, path );
	}

	public void DrawModel( Engine.Model model, Transform transform ) {
		_mainModelShader.Use();
		_mainModelShader.SetUniform( "uTexture", 0 );
		_mainModelShader.SetUniform( "uTransform", transform.Matrix );
		_mainModelShader.SetUniform( "uView", Camera.ViewMatrix );
		_mainModelShader.SetUniform( "uProjection", _currentProjection );

		var backendModel = model.BackendModel as Model ?? throw new Exception( "Bad" );

		foreach ( var mesh in backendModel.Meshes ) {
			mesh.Bind();

			unsafe {
				_gl.DrawElements( PrimitiveType.Triangles, mesh.IndiceCount, DrawElementsType.UnsignedInt, null );
			}
		}
	}
}
