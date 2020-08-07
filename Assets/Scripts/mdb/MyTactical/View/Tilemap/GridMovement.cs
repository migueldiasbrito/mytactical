using UnityEngine;

namespace mdb.MyTactial.View.TilemapView
{
    public class GridMovement : MonoBehaviour
    {
        public Vector2? MoveTo = null;

        [SerializeField]
        private float _speed = 5f;

        void Update()
        {
            if(MoveTo != null)
            {
                transform.position = Vector2.MoveTowards(transform.position, MoveTo.Value, _speed * Time.deltaTime);

                if(Vector2.Distance(transform.position, MoveTo.Value) == 0)
                {
                    MoveTo = null;
                }
            }
        }
    }
}
