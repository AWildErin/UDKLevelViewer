using OpenTK.Mathematics;

namespace UDKLevelViewer.App.Entity
{
	public abstract class Actor
	{
		public Vector3 Position;
		public Quaternion Rotation;

		public Actor()
		{
			Awake();
		}

		~Actor()
		{
			OnDestroy();
		}

		public virtual void Awake() { }
		public virtual void Start() { }

		public virtual void OnDestroy() { }

		/* Update Methods
		 * These are called each tick or on a specified interval */
		public virtual void Update() { }
		public virtual void FixedUpdate() { }
		public virtual void LateUpdate() { }

		/* Misc Methods */
		public virtual void OnEnable() { }
		public virtual void OnDisable() { }
	}
}