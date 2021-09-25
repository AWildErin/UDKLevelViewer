using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using UDKLevelViewer.App.Render;

namespace UDKLevelViewer.App.Core
{
    public class MainWindow : GameWindow
	{
		float[] vertices =
		{
			//Position          Texture coordinates
			 0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
			 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
			-0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
			-0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
		};

		public readonly uint[] indices =
		{
			0, 1, 3,
			1, 2, 3
		};

		public uint[] facesArray;
		public float[] vertsArray;

		private int _vertexBufferObject;
		private int _vertexArrayObject;
		private int _elementBufferObject;

		private Shader _shader;
		private Texture _texture;

		public MainWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
			: base(gameWindowSettings, nativeWindowSettings)
		{
		}

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

			_vertexArrayObject = GL.GenVertexArray();
			GL.BindVertexArray(_vertexArrayObject);

			_vertexBufferObject = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
			GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

			_elementBufferObject = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
			GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

			_shader = new Shader("Data/Shaders/shader.vert", "Data/Shaders/shader.frag");
			_shader.Use();

			var vertexLocation = _shader.GetAttribLocation("aPosition");
			GL.EnableVertexAttribArray(vertexLocation);
			GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

			var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
			GL.EnableVertexAttribArray(texCoordLocation);
			GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

			_texture = Texture.LoadFromFile("data/textures/unreal/DefaultDiffuse.png");
			_texture.Use(TextureUnit.Texture0);
		}

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            _shader.Use();

			GL.BindVertexArray(_vertexArrayObject);

			GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

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
}