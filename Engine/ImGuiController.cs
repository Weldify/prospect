using System;
using System.Linq;
using System.Numerics;
using System.Text;

using Veldrid;
using Veldrid.SPIRV;

using ImGuiNET;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Prospect.Engine;

partial class ImGuiController : IDisposable {
	GraphicsDevice _graphicsDevice;
	DeviceBuffer _vertexBuffer;
	DeviceBuffer _indexBuffer;
	DeviceBuffer _projectionMatrixBuffer;
	Shader _vertexShader;
	Shader _fragmentShader;
	ResourceLayout _resourceLayout;
	ResourceLayout _textureLayout;
	Pipeline _pipeline;
	Texture _fontTexture;
	TextureView _fontTextureView;
	ResourceSet _mainResourceSet;
	ResourceSet _fontTextureResourceSet;
	Vector2i _size;

	readonly IntPtr _fontAtlasId = 1;
	bool _frameBegan = false;

	public ImGuiController( GraphicsDevice graphicsDevice, Vector2i size ) {
		_graphicsDevice = graphicsDevice;
		_size = size;

		ImGui.CreateContext();

		var io = ImGui.GetIO();
		io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
		io.ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard | ImGuiConfigFlags.DockingEnable;
		io.Fonts.Flags |= ImFontAtlasFlags.NoBakedLines;

		createDeviceResources();
	}

	public void Draw( CommandList commandList ) {
		if ( !_frameBegan ) return;
		_frameBegan = false;

		ImGui.Render();
		renderImDrawData( ImGui.GetDrawData(), commandList );
	}

	public void Update( float delta, InputSnapshot snapshot ) {
		if ( _frameBegan )
			ImGui.Render();

		setPerFrameImGuiData( delta );
		updateImGuiInput( snapshot );

		_frameBegan = true;
		ImGui.NewFrame();
	}

	public void Dispose() {
		_vertexBuffer.Dispose();
		_indexBuffer.Dispose();
		_projectionMatrixBuffer.Dispose();
		_fontTexture.Dispose();
		_fontTextureView.Dispose();
		_vertexShader.Dispose();
		_fragmentShader.Dispose();
		_resourceLayout.Dispose();
		_textureLayout.Dispose();
		_pipeline.Dispose();
		_mainResourceSet.Dispose();
	}

	void renderImDrawData( ImDrawDataPtr drawData, CommandList commandList ) {
		uint vertexOffsetInVertices = 0;
		uint indexOffsetInElements = 0;

		if ( drawData.CmdListsCount == 0 ) {
			return;
		}

		uint totalVBSize = (uint)(drawData.TotalVtxCount * Unsafe.SizeOf<ImDrawVert>());
		if ( totalVBSize > _vertexBuffer.SizeInBytes ) {
			_graphicsDevice.DisposeWhenIdle( _vertexBuffer );
			_vertexBuffer = _graphicsDevice.ResourceFactory.CreateBuffer( new BufferDescription( (uint)(totalVBSize * 1.5f), BufferUsage.VertexBuffer | BufferUsage.Dynamic ) );
		}

		uint totalIBSize = (uint)(drawData.TotalIdxCount * sizeof( ushort ));
		if ( totalIBSize > _indexBuffer.SizeInBytes ) {
			_graphicsDevice.DisposeWhenIdle( _indexBuffer );
			_indexBuffer = _graphicsDevice.ResourceFactory.CreateBuffer( new BufferDescription( (uint)(totalIBSize * 1.5f), BufferUsage.IndexBuffer | BufferUsage.Dynamic ) );
		}

		for ( int i = 0; i < drawData.CmdListsCount; i++ ) {
			ImDrawListPtr cmd_list = drawData.CmdListsRange[i];

			commandList.UpdateBuffer(
				_vertexBuffer,
				vertexOffsetInVertices * (uint)Unsafe.SizeOf<ImDrawVert>(),
				cmd_list.VtxBuffer.Data,
				(uint)(cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>()) );

			commandList.UpdateBuffer(
				_indexBuffer,
				indexOffsetInElements * sizeof( ushort ),
				cmd_list.IdxBuffer.Data,
				(uint)(cmd_list.IdxBuffer.Size * sizeof( ushort )) );

			vertexOffsetInVertices += (uint)cmd_list.VtxBuffer.Size;
			indexOffsetInElements += (uint)cmd_list.IdxBuffer.Size;
		}

		// Setup orthographic projection matrix into our constant buffer
		ImGuiIOPtr io = ImGui.GetIO();
		Matrix4x4 mvp = Matrix4x4.CreateOrthographicOffCenter(
			0f,
			io.DisplaySize.X,
			io.DisplaySize.Y,
			0.0f,
			-1.0f,
			1.0f );

		_graphicsDevice.UpdateBuffer( _projectionMatrixBuffer, 0, ref mvp );

		commandList.SetVertexBuffer( 0, _vertexBuffer );
		commandList.SetIndexBuffer( _indexBuffer, IndexFormat.UInt16 );
		commandList.SetPipeline( _pipeline );
		commandList.SetGraphicsResourceSet( 0, _mainResourceSet );

		drawData.ScaleClipRects( io.DisplayFramebufferScale );

		// Render command lists
		int vtx_offset = 0;
		int idx_offset = 0;
		for ( int n = 0; n < drawData.CmdListsCount; n++ ) {
			ImDrawListPtr cmd_list = drawData.CmdListsRange[n];
			for ( int cmd_i = 0; cmd_i < cmd_list.CmdBuffer.Size; cmd_i++ ) {
				ImDrawCmdPtr pcmd = cmd_list.CmdBuffer[cmd_i];
				if ( pcmd.UserCallback != IntPtr.Zero ) {
					throw new NotImplementedException();
				} else {
					if ( pcmd.TextureId != IntPtr.Zero ) {
						if ( pcmd.TextureId == _fontAtlasId ) {
							commandList.SetGraphicsResourceSet( 1, _fontTextureResourceSet );
						}
					}

					commandList.SetScissorRect(
						0,
						(uint)pcmd.ClipRect.X,
						(uint)pcmd.ClipRect.Y,
						(uint)(pcmd.ClipRect.Z - pcmd.ClipRect.X),
						(uint)(pcmd.ClipRect.W - pcmd.ClipRect.Y) );

					commandList.DrawIndexed( pcmd.ElemCount, 1, pcmd.IdxOffset + (uint)idx_offset, (int)pcmd.VtxOffset + vtx_offset, 0 );
				}
			}
			vtx_offset += cmd_list.VtxBuffer.Size;
			idx_offset += cmd_list.IdxBuffer.Size;
		}
	}

	void setPerFrameImGuiData( float delta ) {
		var io = ImGui.GetIO();
		io.DisplaySize = new Vector2( _size.X, _size.Y );
		io.DeltaTime = delta;
	}

	static void updateImGuiInput( InputSnapshot snapshot ) {
		var io = ImGui.GetIO();
		io.AddMousePosEvent( snapshot.MousePosition.X, snapshot.MousePosition.Y );
		io.AddMouseButtonEvent( 0, snapshot.IsMouseDown( MouseButton.Left ) );
		io.AddMouseButtonEvent( 1, snapshot.IsMouseDown( MouseButton.Right ) );
		io.AddMouseButtonEvent( 2, snapshot.IsMouseDown( MouseButton.Middle ) );
		io.AddMouseButtonEvent( 3, snapshot.IsMouseDown( MouseButton.Button1 ) );
		io.AddMouseButtonEvent( 4, snapshot.IsMouseDown( MouseButton.Button2 ) );
		io.AddMouseWheelEvent( 0f, snapshot.WheelDelta );

		for ( int i = 0; i < snapshot.KeyCharPresses.Count; i++ ) {
			io.AddInputCharacter( snapshot.KeyCharPresses[i] );
		}

		for ( int i = 0; i < snapshot.KeyEvents.Count; i++ ) {
			KeyEvent keyEvent = snapshot.KeyEvents[i];
			if ( tryMapKey( keyEvent.Key, out ImGuiKey imguikey ) ) {
				io.AddKeyEvent( imguikey, keyEvent.Down );
			}
		}
	}

	static bool tryMapKey( Key key, out ImGuiKey result ) {
		static ImGuiKey keyToImGuiKeyShortcut( Key keyToConvert, Key startKey1, ImGuiKey startKey2 ) {
			int changeFromStart1 = (int)keyToConvert - (int)startKey1;
			return startKey2 + changeFromStart1;
		}

		result = key switch {
			>= Key.F1 and <= Key.F12 => keyToImGuiKeyShortcut( key, Key.F1, ImGuiKey.F1 ),
			>= Key.Keypad0 and <= Key.Keypad9 => keyToImGuiKeyShortcut( key, Key.Keypad0, ImGuiKey.Keypad0 ),
			>= Key.A and <= Key.Z => keyToImGuiKeyShortcut( key, Key.A, ImGuiKey.A ),
			>= Key.Number0 and <= Key.Number9 => keyToImGuiKeyShortcut( key, Key.Number0, ImGuiKey._0 ),
			Key.ShiftLeft or Key.ShiftRight => ImGuiKey.ModShift,
			Key.ControlLeft or Key.ControlRight => ImGuiKey.ModCtrl,
			Key.AltLeft or Key.AltRight => ImGuiKey.ModAlt,
			Key.WinLeft or Key.WinRight => ImGuiKey.ModSuper,
			Key.Menu => ImGuiKey.Menu,
			Key.Up => ImGuiKey.UpArrow,
			Key.Down => ImGuiKey.DownArrow,
			Key.Left => ImGuiKey.LeftArrow,
			Key.Right => ImGuiKey.RightArrow,
			Key.Enter => ImGuiKey.Enter,
			Key.Escape => ImGuiKey.Escape,
			Key.Space => ImGuiKey.Space,
			Key.Tab => ImGuiKey.Tab,
			Key.BackSpace => ImGuiKey.Backspace,
			Key.Insert => ImGuiKey.Insert,
			Key.Delete => ImGuiKey.Delete,
			Key.PageUp => ImGuiKey.PageUp,
			Key.PageDown => ImGuiKey.PageDown,
			Key.Home => ImGuiKey.Home,
			Key.End => ImGuiKey.End,
			Key.CapsLock => ImGuiKey.CapsLock,
			Key.ScrollLock => ImGuiKey.ScrollLock,
			Key.PrintScreen => ImGuiKey.PrintScreen,
			Key.Pause => ImGuiKey.Pause,
			Key.NumLock => ImGuiKey.NumLock,
			Key.KeypadDivide => ImGuiKey.KeypadDivide,
			Key.KeypadMultiply => ImGuiKey.KeypadMultiply,
			Key.KeypadSubtract => ImGuiKey.KeypadSubtract,
			Key.KeypadAdd => ImGuiKey.KeypadAdd,
			Key.KeypadDecimal => ImGuiKey.KeypadDecimal,
			Key.KeypadEnter => ImGuiKey.KeypadEnter,
			Key.Tilde => ImGuiKey.GraveAccent,
			Key.Minus => ImGuiKey.Minus,
			Key.Plus => ImGuiKey.Equal,
			Key.BracketLeft => ImGuiKey.LeftBracket,
			Key.BracketRight => ImGuiKey.RightBracket,
			Key.Semicolon => ImGuiKey.Semicolon,
			Key.Quote => ImGuiKey.Apostrophe,
			Key.Comma => ImGuiKey.Comma,
			Key.Period => ImGuiKey.Period,
			Key.Slash => ImGuiKey.Slash,
			Key.BackSlash or Key.NonUSBackSlash => ImGuiKey.Backslash,
			_ => ImGuiKey.None
		};

		return result != ImGuiKey.None;
	}

	void createDeviceResources() {
		var factory = _graphicsDevice.ResourceFactory;

		_vertexBuffer = factory.CreateBuffer( new( 8192, BufferUsage.VertexBuffer | BufferUsage.Dynamic ) );
		_vertexBuffer.Name = "ImGui vertex buffer";

		_indexBuffer = factory.CreateBuffer( new( 2048, BufferUsage.IndexBuffer | BufferUsage.Dynamic ) );
		_indexBuffer.Name = "ImGui index buffer";

		recreateFontDeviceTexture();

		_projectionMatrixBuffer = factory.CreateBuffer( new( 64, BufferUsage.UniformBuffer | BufferUsage.Dynamic ) );
		_projectionMatrixBuffer.Name = "ImGui projection matrix buffer";

		ShaderDescription vertexDescription = new(
				ShaderStages.Vertex,
				Encoding.UTF8.GetBytes( VERTEX_SHADER_CODE ),
				"main"
			);

		ShaderDescription fragmentDescription = new(
			ShaderStages.Fragment,
			Encoding.UTF8.GetBytes( FRAGMENT_SHADER_CODE ),
			"main"
		);

		var shaders = factory.CreateFromSpirv( vertexDescription, fragmentDescription );
		_vertexShader = shaders[0];
		_fragmentShader = shaders[1];

		VertexLayoutDescription vertexLayout = new(
			new VertexElementDescription( "in_position", VertexElementSemantic.Position, VertexElementFormat.Float2 ),
			new VertexElementDescription( "in_texCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2 ),
			new VertexElementDescription( "in_color", VertexElementSemantic.Color, VertexElementFormat.Byte4_Norm )
		);

		_resourceLayout = factory.CreateResourceLayout( new(
			new( "ProjectionMatrixBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex ),
			new( "MainSampler", ResourceKind.Sampler, ShaderStages.Fragment )
		) );

		_textureLayout = factory.CreateResourceLayout( new ResourceLayoutDescription(
			new ResourceLayoutElementDescription( "MainTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment )
		) );

		GraphicsPipelineDescription pipelineDescription = new() {
			BlendState = BlendStateDescription.SingleAlphaBlend,
			DepthStencilState = new( false, false, ComparisonKind.Always ),
			RasterizerState = new( FaceCullMode.None, PolygonFillMode.Solid, FrontFace.Clockwise, false, true ),
			ShaderSet = new( new[] { vertexLayout }, new[] { _vertexShader, _fragmentShader } ),
			ResourceLayouts = new[] { _resourceLayout, _textureLayout },
			Outputs = _graphicsDevice.SwapchainFramebuffer.OutputDescription,
		};

		_pipeline = factory.CreateGraphicsPipeline( pipelineDescription );

		_mainResourceSet = factory.CreateResourceSet( new(
			_resourceLayout, _projectionMatrixBuffer, _graphicsDevice.PointSampler
		) );

		_fontTextureResourceSet = factory.CreateResourceSet( new( _textureLayout, _fontTextureView ) );
	}

	void recreateFontDeviceTexture() {
		var io = ImGui.GetIO();

		io.Fonts.GetTexDataAsRGBA32( out IntPtr pixels, out int width, out int height, out int bytesPerPixel );
		io.Fonts.SetTexID( _fontAtlasId );

		_fontTexture = _graphicsDevice.ResourceFactory.CreateTexture(
			TextureDescription.Texture2D(
				(uint)width,
				(uint)height,
				1,
				1,
				PixelFormat.B8_G8_R8_A8_UNorm,
				TextureUsage.Sampled
			)
		);
		_fontTexture.Name = "ImGui font texture";

		_graphicsDevice.UpdateTexture(
			_fontTexture,
			pixels,
			(uint)(bytesPerPixel * width * height),
			0, 0, 0,
			(uint)width, (uint)height,
			1, 0, 0
		);

		_fontTextureView = _graphicsDevice.ResourceFactory.CreateTextureView( _fontTexture );

		io.Fonts.ClearTexData();
	}
}