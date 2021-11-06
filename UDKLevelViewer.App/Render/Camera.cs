using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDKLevelViewer.App.Render
{
	public class Camera
	{
		public Vector3 Position { get; set; }

		private Vector3 _front = -Vector3.UnitZ;
		public Vector3 Front => _front;
		public Vector3 Up { get; private set; } = Vector3.UnitY;
		public Vector3 Right { get; private set; } = Vector3.UnitX;

		private float _pitch;
		public float Pitch
		{
			get => MathHelper.RadiansToDegrees(_pitch);
			set
			{
				float angle = MathHelper.Clamp(value, -89.0f, 89.0f);
				_pitch = MathHelper.DegreesToRadians(angle);
				UpdateVectors();
			}
		}

		// We set a default value here as we would always start
		// 90 degrees right.
		private float _yaw = -MathHelper.PiOver2;
		public float Yaw
		{
			get => MathHelper.RadiansToDegrees(_yaw);
			set
			{
				_yaw = MathHelper.DegreesToRadians(value);
				UpdateVectors();
			}
		}

		// Defaults to 90f
		private float _fov = MathHelper.PiOver2;
		public float Fov
		{
			get => MathHelper.RadiansToDegrees(_fov);
			set
			{
				float angle = MathHelper.Clamp(value, 1f, 90f);
				Console.WriteLine($"FOV is {angle}");
				_fov = MathHelper.DegreesToRadians(angle);
			}
		}

		public float NearPlane { get; set; }
		public float FarPlane { get; set; }

		public float AspectRatio { private get; set; }

		public Matrix4 ProjMatrix { get; private set; }
		public Matrix4 ViewMatrix { get; private set; }

		public Camera(Vector3 pos, float aspectRatio, float nearPlane = 0.01f, float farPlane = 8149f)
		{
			Position = pos;
			AspectRatio = aspectRatio;
			NearPlane = nearPlane;
			FarPlane = farPlane;

			UpdateProjectionMatrix();
			UpdateViewMatrix();
		}

		public void UpdateProjectionMatrix()
		{
			ProjMatrix = Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, NearPlane, FarPlane);
			Shader.SetProjectionMatrix(ProjMatrix);
		}

		public void UpdateViewMatrix()
		{
			ViewMatrix = Matrix4.LookAt(Position, Position + _front, Up);
			Shader.SetProjectionMatrix(ViewMatrix);
		}

		public void UpdateVectors()
		{
			_front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
			_front.Y = MathF.Sin(_pitch);
			_front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

			_front = Vector3.Normalize(_front);
			Right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
			Up = Vector3.Normalize(Vector3.Cross(Right, _front));
		}
	}
}
