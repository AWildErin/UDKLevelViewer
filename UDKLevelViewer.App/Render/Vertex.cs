using OpenTK.Mathematics;

namespace UDKLevelViewer.App.Render
{
	public readonly struct Vertex
	{
		public Vertex(float x, float y, float z, float u = 0.0f, float v = 1.0f)
		{
			Position = new Vector3(x, y, z);
			Uv = new Vector2(u, v);
		}

		public Vector3 Position { get; }
		public Vector2 Uv { get; }
	}
}
