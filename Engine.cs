using System;
using System.Diagnostics;
using System.Numerics;
using System.Text;

using Veldrid;
using Veldrid.StartupUtilities;
using Veldrid.Sdl2;
using Veldrid.SPIRV;

using ImGuiNET;

namespace Prospect;

partial class Engine {
	static CommandList _commandList;
	static DeviceBuffer _vertexBuffer;
	static DeviceBuffer _indexBuffer;
	static Shader _vertexShader;
	static Shader _fragmentShader;
	static Pipeline _pipeline;
	static GraphicsDevice _graphicsDevice;
	static Sdl2Window _mainWindow;
	static ImGuiController _imGuiController;

	static void Main() {
		createMainWindow();
		createGraphicsDevice();
		createResources();
		createImGuiController();

		var stopWatch = Stopwatch.StartNew();
		float delta;

		while ( _mainWindow.Exists ) {
			delta = stopWatch.ElapsedTicks / (float)Stopwatch.Frequency;
			stopWatch.Restart();

			InputSnapshot inputSnapshot = _mainWindow.PumpEvents();
			if ( !_mainWindow.Exists ) break;

			_imGuiController.Update( delta, inputSnapshot );
			submitUi();

			draw();
		}

		disposeResources();
	}

	static void draw() {
		_commandList.Begin();
		_commandList.SetFramebuffer( _graphicsDevice.SwapchainFramebuffer );

		_commandList.ClearColorTarget( 0, RgbaFloat.Black );

		_imGuiController.Draw( _commandList );

		_commandList.End();

		_graphicsDevice.SubmitCommands( _commandList );
		_graphicsDevice.SwapBuffers();
	}

	static bool isCaming = false;

	static void submitUi() {
		ImGui.Text( "I am gooey" );

		if ( ImGui.Button( "Are we batakam?" ) )
			isCaming = !isCaming;

		if ( isCaming )
			ImGui.Text( "OO MAI GOT!!! AMKAMIN@GGG" );
	}

	static void createMainWindow() {
		WindowCreateInfo windowCi = new() {
			X = 100,
			Y = 100,
			WindowWidth = 960,
			WindowHeight = 540,
			WindowTitle = "Prospect"
		};

		_mainWindow = VeldridStartup.CreateWindow( ref windowCi );
	}

	static void createGraphicsDevice() {
		GraphicsDeviceOptions options = new() {
			PreferDepthRangeZeroToOne = true,
			PreferStandardClipSpaceYDirection = true
		};

		_graphicsDevice = VeldridStartup.CreateGraphicsDevice( _mainWindow, options, GraphicsBackend.Vulkan );
	}

	static void createResources() {
		var factory = _graphicsDevice.ResourceFactory;

		VertexPositionColor[] quadVertices = {
			new VertexPositionColor(new Vector2(-0.75f, 0.75f), RgbaFloat.Red),
			new VertexPositionColor(new Vector2(0.75f, 0.75f), RgbaFloat.Green),
			new VertexPositionColor(new Vector2(-0.75f, -0.75f), RgbaFloat.Blue),
			new VertexPositionColor(new Vector2(0.75f, -0.75f), RgbaFloat.Yellow),
		};

		ushort[] quadIndices = { 0, 1, 2, 3 };

		_vertexBuffer = factory.CreateBuffer( new( 4 * VertexPositionColor.SIZE_IN_BYTES, BufferUsage.VertexBuffer ) );
		_indexBuffer = factory.CreateBuffer( new( 4 * sizeof( ushort ), BufferUsage.IndexBuffer ) );

		_graphicsDevice.UpdateBuffer( _vertexBuffer, 0, quadVertices );
		_graphicsDevice.UpdateBuffer( _indexBuffer, 0, quadIndices );

		VertexLayoutDescription vertexLayout = new(
			new VertexElementDescription( "Position", VertexElementSemantic.Position, VertexElementFormat.Float2 ),
			new VertexElementDescription( "Color", VertexElementSemantic.Color, VertexElementFormat.Float4 )
		);

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
		_fragmentShader = shaders[0];

		GraphicsPipelineDescription pipelineDescription = new() {
			BlendState = BlendStateDescription.SingleOverrideBlend,
			DepthStencilState = new(
				depthTestEnabled: true,
				depthWriteEnabled: true,
				comparisonKind: ComparisonKind.LessEqual
			),
			RasterizerState = new RasterizerStateDescription(
				cullMode: FaceCullMode.Back,
				fillMode: PolygonFillMode.Solid,
				frontFace: FrontFace.Clockwise,
				depthClipEnabled: true,
				scissorTestEnabled: false
			),
			PrimitiveTopology = PrimitiveTopology.TriangleStrip,
			ResourceLayouts = Array.Empty<ResourceLayout>(),
			ShaderSet = new(
				vertexLayouts: new[] { vertexLayout },
				shaders: shaders
			),
			Outputs = _graphicsDevice.SwapchainFramebuffer.OutputDescription
		};

		_pipeline = factory.CreateGraphicsPipeline( pipelineDescription );
		_commandList = factory.CreateCommandList();
	}

	static void createImGuiController() {
		_imGuiController = new( _graphicsDevice, new Vector2i( _mainWindow.Width, _mainWindow.Height ) );
	}

	static void disposeResources() {
		_imGuiController.Dispose();

		_pipeline.Dispose();

		_vertexShader.Dispose(); // Vertex shader
		_fragmentShader.Dispose(); // Fragment shader

		_vertexBuffer.Dispose();
		_indexBuffer.Dispose();
		_commandList.Dispose();

		_mainWindow.Close();
		_graphicsDevice.Dispose();
	}
}