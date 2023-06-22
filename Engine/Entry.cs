using System;
using System.Diagnostics;
using System.Numerics;
using System.Text;

using Veldrid;
using Veldrid.StartupUtilities;
using Veldrid.Sdl2;
using Veldrid.SPIRV;

using ImGuiNET;
using System.Collections.Generic;

namespace Prospect.Engine;

public static partial class Entry {
	public static Window MainWindow { get; private set; } = null!;

	static List<IGame> _games = new();

	static CommandList _commandList;
	static DeviceBuffer _vertexBuffer;
	static DeviceBuffer _indexBuffer;
	static Shader _vertexShader;
	static Shader _fragmentShader;
	static Pipeline _pipeline;
	static GraphicsDevice _graphicsDevice;
	static ImGuiController _imGuiController;

	public static void Run<T>() where T : IGame, new() {
		T game = new();
		game.Start();

		_games.Add( game );

		// First game to start is the main game
		if ( _games.Count != 1 ) return;

		MainWindow = new( Vector2u.Zero, new( 200, 200 ), "MainWindow" );
		MainLoop();
	}

	static void MainLoop() {
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