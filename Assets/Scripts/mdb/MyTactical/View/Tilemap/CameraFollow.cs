using UnityEngine;

namespace mdb.MyTactial.View.TilemapView
{
	public class CameraFollow : MonoBehaviour
	{
		public static CameraFollow instance;
		public GameObject Target { get; set; }

		public Vector2 SmoothTime = new Vector2(5f, 5f);

		private Vector2 velocity;

		private void Awake()
		{
			instance = this;
		}

		private void FixedUpdate()
		{
			if (Target != null)
			{
				float x = Mathf.SmoothDamp(transform.position.x, Target.transform.position.x, ref velocity.x, SmoothTime.x);
				float y = Mathf.SmoothDamp(transform.position.y, Target.transform.position.y, ref velocity.y, SmoothTime.y);

				transform.position = new Vector3(x, y, transform.position.z);
			}
		}
	}
}