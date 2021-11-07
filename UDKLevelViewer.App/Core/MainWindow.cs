using LegendaryExplorerCore;
using LegendaryExplorerCore.Packages;
using LegendaryExplorerCore.Unreal;
using LegendaryExplorerCore.Unreal.BinaryConverters;
using LegendaryExplorerCore.Unreal.Classes;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UDKLevelViewer.App.Entity;
using UDKLevelViewer.App.Render;
using Shader = UDKLevelViewer.App.Render.Shader;

namespace UDKLevelViewer.App.Core
{
	public class MainWindow : GameWindow
	{
		float[] vertices =
		{
			//Position          Texture coordinates
			//0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
			//0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
			//-0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
			//-0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
		};

		public uint[] indices =
		{
			//0, 1, 3,
			//1, 2, 3
		};

		public uint[] facesArray;
		public float[] vertsArray;

		private int _vertexBufferObject;
		private int _vertexArrayObject;
		private int _elementBufferObject;

		private Shader _shader;
		private Texture _texture;

		private IMEPackage package;

		private Camera camera = null;

		private bool _firstMove = true;
		private Vector2 _lastPos;
		double _time;

		private StaticMeshActor sm1;
		private StaticMeshActor sm2;

		public MainWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
			: base(gameWindowSettings, nativeWindowSettings)
		{
		}

        protected override void OnLoad()
        {
            base.OnLoad();

			GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
			GL.Enable(EnableCap.DepthTest);

			var sc = new SynchronizationContext();
			SynchronizationContext.SetSynchronizationContext(sc);
			LegendaryExplorerCoreLib.InitLib(TaskScheduler.FromCurrentSynchronizationContext(), x => { });

			string filePath = @"D:\Steam Games\steamapps\common\Outlast\OLGame\CookedPCConsole\Lab_03.upk";
			//string filePath = @"D:\Steam Games\steamapps\common\Outlast\OLGame\CookedPCConsole\OLGame.upk";
			package = MEPackageHandler.OpenMEPackage(filePath, forceLoadFromDisk: true);

			// Require lab03
			var model = ObjectBinary.From<StaticMesh>(package.GetUExport(4272)); // Doorway
																				 //var model = ObjectBinary.From<StaticMesh>(package.GetUExport(4265)); // Airlock
			var lod = model.LODModels[0];
			var test = new List<float>();

			#region OldCode
			// Skel Mesh
			/*
			foreach (var vert in lod.VertexBufferGPUSkin.VertexData)
			{
				var v = vert.Position;
				var uv = vert.UV;

				test.Add(-v.X / 20);
				test.Add(v.Z / 20);
				test.Add(v.Y / 20);
				test.Add(uv.X);
				test.Add(uv.Y);
			}
			*/

			
			for (int i = 0; i < lod.NumVertices; i++)
			{
				var v = lod.PositionVertexBuffer.VertexData[i];
				var uv = lod.VertexBuffer.VertexData[i].HalfPrecisionUVs;

				test.Add(-v.X);
				test.Add(v.Z);
				test.Add(v.Y);
				test.Add(uv[0].X);
				test.Add(uv[0].Y);
			}
			vertices = test.ToArray();

			var test2 = new List<uint>();
			if (lod.IndexBuffer.Length > 0)
			{
				for (int i = 0; i < lod.IndexBuffer.Length; i += 3)
				{
					test2.Add(lod.IndexBuffer[i]);
					test2.Add(lod.IndexBuffer[i + 1]);
					test2.Add(lod.IndexBuffer[i + 2]);
				}
			}
			indices = test2.ToArray();

			/*
			_vertexArrayObject = GL.GenVertexArray();
			GL.BindVertexArray(_vertexArrayObject);

			_vertexBufferObject = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
			GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

			_elementBufferObject = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
			GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

			//_shader = new Shader("Data/Shaders/shader.vert", "Data/Shaders/shader.frag");
			_shader = new Shader("Data/Shaders/unreal.vert", "Data/Shaders/unreal.frag");
			_shader.Bind();

			_shader.SetMatrix4("model", Matrix4.CreateTranslation(new Vector3(0,0,0)));
			//_shader.SetMatrix4("projMatrix", Matrix4.Identity);
			//_shader.SetMatrix4("viewMatrix", Matrix4.Identity);

			var vertexLocation = _shader.GetAttribLocation("aPosition");
			GL.EnableVertexAttribArray(vertexLocation);
			GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

			var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
			GL.EnableVertexAttribArray(texCoordLocation);
			GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

			Texture2D tex = new Texture2D(package.GetUExport(6153));

			//_texture = Texture.LoadFromFile("data/textures/unreal/DefaultDiffuse.png");
			_texture = Texture.LoadFromTexture2D(tex);
			_texture.Use(TextureUnit.Texture0);
			*/
			#endregion OldCode

			sm1 = StaticMeshActor.CreateFromStaticMesh(model, new Vector3(0, 0, 0), new Vector3(0, 0, 0));
			//sm1.shader.SetMatrix4("model", Matrix4.CreateTranslation(new Vector3(0, 0, 0)));

			//sm2 = StaticMeshActor.CreateFromStaticMesh(model, Vector3.Zero, Vector3.Zero);
			//sm2.shader.SetMatrix4("model", Matrix4.CreateTranslation(new Vector3(200, 0, 0)));

			//_shader.SetMatrix4("model", Matrix4.CreateTranslation(new Vector3(0, 0, 0)));

			camera = new Camera(Vector3.UnitZ, Size.X / (float)Size.Y);

			CursorGrabbed = true;
		}

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

			_time += 4.0 * e.Time;

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			//GL.BindVertexArray(_vertexArrayObject);

			//var model = Matrix4.Identity * Matrix4.CreateScale(0.1f);// * Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(_time));
			//_shader.SetMatrix4("model", model);
			//camera.UpdateViewMatrix();
			//camera.UpdateProjectionMatrix();
			//_shader.SetMatrix4("viewMatrix", camera.ViewMatrix);
			//_shader.SetMatrix4("projMatrix", camera.ProjMatrix);

			camera.UpdateViewMatrix();
			camera.UpdateProjectionMatrix();

			sm1.RenderTest(camera.ViewMatrix, camera.ProjMatrix);
			//sm2.RenderTest(camera.ViewMatrix, camera.ProjMatrix);

			//GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

			SwapBuffers();
		}

        protected override void OnUpdateFrame(FrameEventArgs e)
		{
			base.OnUpdateFrame(e);

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

			const float cameraSpeed = 1.5f;
			const float sensitivity = 0.2f;

			if (KeyboardState.IsKeyDown(Keys.W))
			{
				camera.Position += camera.Front * cameraSpeed * (float)e.Time; // Forward
			}

			if (KeyboardState.IsKeyDown(Keys.S))
			{
				camera.Position -= camera.Front * cameraSpeed * (float)e.Time; // Backwards
			}
			if (KeyboardState.IsKeyDown(Keys.A))
			{
				camera.Position -= camera.Right * cameraSpeed * (float)e.Time; // Left
			}
			if (KeyboardState.IsKeyDown(Keys.D))
			{
				camera.Position += camera.Right * cameraSpeed * (float)e.Time; // Right
			}
			if (KeyboardState.IsKeyDown(Keys.Space))
			{
				camera.Position += camera.Up * cameraSpeed * (float)e.Time; // Up
			}
			if (KeyboardState.IsKeyDown(Keys.LeftShift))
			{
				camera.Position -= camera.Up * cameraSpeed * (float)e.Time; // Down
			}

			if (KeyboardState.IsKeyPressed(Keys.F4))
			{
				camera.Position = Vector3.Zero;
			}

			var mouse = MouseState;
			if (_firstMove) // This bool variable is initially set to true.
			{
				_lastPos = new Vector2(mouse.X, mouse.Y);
				_firstMove = false;
			}
			else
			{
				// Calculate the offset of the mouse position
				var deltaX = mouse.X - _lastPos.X;
				var deltaY = mouse.Y - _lastPos.Y;
				_lastPos = new Vector2(mouse.X, mouse.Y);

				// Apply the camera pitch and yaw (we clamp the pitch in the camera class)
				camera.Yaw += deltaX * sensitivity;
				camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
			}

			if (KeyboardState.IsKeyDown(Keys.LeftBracket))
			{
				sm1.Position.Y += 0.1f;

				Console.WriteLine($"SM1 Pos: {sm1.Position}");
			}

			if (KeyboardState.IsKeyDown(Keys.RightBracket))
			{
				sm1.Position.Y -= 0.1f;
				Console.WriteLine($"SM1 Pos: {sm1.Position}");
			}

			if (KeyboardState.IsKeyDown(Keys.Comma))
			{
				var rot = sm1.Rotation.ToEulerAngles();
				rot.X += 0.01f;

				sm1.Rotation = Quaternion.FromEulerAngles(rot);
				Console.WriteLine($"SM1 Rot: {sm1.Rotation.ToEulerAngles()}");
			}

			if (KeyboardState.IsKeyDown(Keys.Period))
			{
				var rot = sm1.Rotation.ToEulerAngles();
				rot.X -= 0.01f;

				sm1.Rotation = Quaternion.FromEulerAngles(rot);
				Console.WriteLine($"SM1 Rot: {sm1.Rotation.ToEulerAngles()}");
			}
		}
		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			base.OnMouseWheel(e);

			camera.Fov -= e.OffsetY;
		}

		protected override void OnResize(ResizeEventArgs e)
		{
			base.OnResize(e);

			GL.Viewport(0, 0, Size.X, Size.Y);

			camera.UpdateProjectionMatrix();
		}
	}
}