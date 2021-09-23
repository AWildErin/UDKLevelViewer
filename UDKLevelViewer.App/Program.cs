using System;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using UDKLevelViewer.App.Render;

namespace UDKLevelViewer.App
{
	public class Window : GameWindow
	{
		private readonly float[] _vertices =
		{
			-0.5f, -0.5f, 0.0f, // Bottom-left vertex
             0.5f, -0.5f, 0.0f, // Bottom-right vertex
             0.0f,  0.5f, 0.0f  // Top vertex
        };

		private int _vertexBufferObject;

		private int _vertexArrayObject;

		private Shader _shader;

		public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
			: base(gameWindowSettings, nativeWindowSettings)
		{
		}

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            _vertexBufferObject = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);

            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            GL.EnableVertexAttribArray(0);

            _shader = new Shader("Data/Shaders/shader.vert", "Data/Shaders/shader.frag");

            _shader.Use();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            _shader.Use();

            GL.BindVertexArray(_vertexArrayObject);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            SwapBuffers();
		}

        protected override void OnUpdateFrame(FrameEventArgs e)
		{
			if (KeyboardState.IsKeyDown(Keys.Escape))
			{
				Close();
			}

			if (KeyboardState.IsKeyPressed(Keys.F1))
			{
				GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
			}

			if (KeyboardState.IsKeyPressed(Keys.F2))
			{
				GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
			}

			if (KeyboardState.IsKeyPressed(Keys.F3))
			{
				GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Point);
			}

			base.OnUpdateFrame(e);
		}

		protected override void OnResize(ResizeEventArgs e)
		{
			base.OnResize(e);

			GL.Viewport(0, 0, Size.X, Size.Y);
		}
	}

	public static class Program
	{
		private static void Main(string[] args)
		{
			var nativeWindowSettings = new NativeWindowSettings()
			{
				Size = new Vector2i(800, 600),
				Title = "UDK Level Viewer"
			};

			using (var window = new Window(GameWindowSettings.Default, nativeWindowSettings))
			{
				window.Run();
			}
		}
	}
}
