using LegendaryExplorerCore.Unreal.BinaryConverters;
using LegendaryExplorerCore.Unreal.Classes;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using UDKLevelViewer.App.Render;
using Shader = UDKLevelViewer.App.Render.Shader;

namespace UDKLevelViewer.App.Entity
{
	// @todo: probably move mesh related stuff to their own class so we can easily move this to other types of mesh actors
	// @todo: possibly look into adding LODs
	// @todo: allow meshes to have multiple materials
	public class StaticMeshActor : Actor
	{
		private readonly List<uint> _indices = new List<uint>();
		private readonly List<Vertex> _vertices = new List<Vertex>();

		public float[] VertexArrayCache { get; private set; } = Array.Empty<float>();
		public uint[] IndexArrayCache { get; private set; } = Array.Empty<uint>();

		private int VertexBufferId;
		private int VertexArrayId;
		private int ElementBufferId;

		private uint CurrentIndex;

		public bool Finalized;

		public Shader shader;
		public Texture texture;

		public StaticMeshActor()
		{

		}

		public void BakeMesh()
		{
			// @todo: probably doing this is bad, but for now it'll pass
			// We mutliply by 5 as that is how much data we will pass through
			VertexArrayCache = new float[_vertices.Count * 5];
			for (int i = 0; i < _vertices.Count; i++)
			{
				var vertex = _vertices[i];
				VertexArrayCache[i * 5] = vertex.Position.X;
				VertexArrayCache[i * 5 + 1] = vertex.Position.Y;
				VertexArrayCache[i * 5 + 2] = vertex.Position.Z;
				VertexArrayCache[i * 5 + 3] = vertex.Uv.X;
				VertexArrayCache[i * 5 + 4] = vertex.Uv.Y;
			}

			// Check if the model already supplied it's own indices
			if (IndexArrayCache.Length < 1)
			{
				IndexArrayCache = new uint[_indices.Count];
				for (var i = 0; i < _indices.Count; i++)
				{
					IndexArrayCache[i] = _indices[i];
				}
			}

			// Create the vertex buffer
			VertexBufferId = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferId);
			GL.BufferData(BufferTarget.ArrayBuffer, VertexArrayCache.Length * sizeof(float), VertexArrayCache, BufferUsageHint.StaticDraw);
			VertexArrayId = GL.GenVertexArray();
			GL.BindVertexArray(VertexArrayId);

			// Create the element buffer
			ElementBufferId = GL.GenBuffer();
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferId);
			GL.BufferData(BufferTarget.ElementArrayBuffer, IndexArrayCache.Length * sizeof(uint), IndexArrayCache, BufferUsageHint.StaticDraw);

			shader = new Shader("Data/Shaders/unreal.vert", "Data/Shaders/unreal.frag");
			shader.Bind();

			//shader.SetMatrix4("model", Matrix4.CreateTranslation(new Vector3(0, 0, 0)));
			//shader.SetMatrix4("viewMatrix", Matrix4.Identity);
			//shader.SetMatrix4("projMatrix", Matrix4.Identity);

			GL.EnableVertexAttribArray(shader.GetAttribLocation("aPosition"));
			GL.VertexAttribPointer(shader.GetAttribLocation("aPosition"), 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
			GL.EnableVertexAttribArray(shader.GetAttribLocation("aTexCoord"));
			GL.VertexAttribPointer(shader.GetAttribLocation("aTexCoord"), 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

			texture = Texture.LoadFromFile("data/textures/unreal/DefaultDiffuse.png");
			texture.Use(TextureUnit.Texture0);
		}

		// @todo: Remove this method, it is here temporarly
		public void RenderTest(Matrix4 mat1, Matrix4 mat2)
		{
			var location = Matrix4.CreateTranslation(Position);
			var rotation = Matrix4.CreateFromQuaternion(Rotation);
			var scale = Matrix4.CreateScale(0.01f);

			GL.BindVertexArray(VertexArrayId);
			texture.Use(TextureUnit.Texture0);

			shader.SetMatrix4("model", Matrix4.Identity * rotation * location); //* scale);
			shader.SetMatrix4("viewMatrix", mat1);
			shader.SetMatrix4("projMatrix", mat2);

			//shader.Bind();
			GL.DrawElements(PrimitiveType.Triangles, IndexArrayCache.Length, DrawElementsType.UnsignedInt, 0);
		}

		// @todo: look at readding back this if statement, current breaks the rendering.
		public void AddVertex(Vertex vert)
		{
			_vertices.Add(vert);
			_indices.Add(CurrentIndex);
			CurrentIndex++;

			/*
			if (_vertices.Contains(vert))
			{
				_indices.Add((uint)_vertices.IndexOf(vert));
			}
			else
			{
				_vertices.Add(vert);
				_indices.Add(CurrentIndex);
				CurrentIndex++;
			}
			*/
		}

		public static StaticMeshActor CreateFromStaticMesh(StaticMesh mesh, Vector3 Position, Vector3 Rotation)
		{
			var actor = new StaticMeshActor();
			var lod = mesh.LODModels[0];

			// Parse the mesh vertex buffer
			for (int i = 0; i < lod.NumVertices; i++)
			{
				var v = lod.PositionVertexBuffer.VertexData[i];
				if (lod.VertexBuffer.bUseFullPrecisionUVs)
				{
					var uv = lod.VertexBuffer.VertexData[i].FullPrecisionUVs;
					actor.AddVertex(new Vertex(-v.X, v.Z, v.Y, uv[0].X, uv[0].Y));
				}
				else
				{
					var uv = lod.VertexBuffer.VertexData[i].HalfPrecisionUVs;
					actor.AddVertex(new Vertex(-v.X, v.Z, v.Y, uv[0].X, uv[0].Y));
				}
			}

			// Parse the mesh index buffer
			actor.IndexArrayCache = new uint[lod.IndexBuffer.Length];
			// Todo: check if index buffer exists, some meshes don't have it.
			if (lod.IndexBuffer.Length > 0)
			{
				for (int i = 0; i < lod.IndexBuffer.Length; i += 3)
				{
					actor.IndexArrayCache[i] = lod.IndexBuffer[i];
					actor.IndexArrayCache[i + 1] = lod.IndexBuffer[i + 1];
					actor.IndexArrayCache[i + 2] = lod.IndexBuffer[i + 2];
				}
			}

			actor.BakeMesh();

			actor.Position = Position;

			// @todo: find a better way to do this
			Rotation.X = MathHelper.DegreesToRadians(Rotation.X);
			Rotation.Y = MathHelper.DegreesToRadians(Rotation.Y);
			Rotation.Z = MathHelper.DegreesToRadians(Rotation.Z);

			actor.Rotation = Quaternion.FromEulerAngles(Rotation);

			return actor;
		}

		public static StaticMeshActor CreateFromSkeletalMesh(SkeletalMesh mesh, Vector3 Position, Vector3 Rotation)
		{
			return null;
		}
	}
}