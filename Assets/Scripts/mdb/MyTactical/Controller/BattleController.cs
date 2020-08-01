using UnityEngine;

using mdb.MyTactial.Model;

namespace mdb.MyTactial.Controller
{
    public class BattleController : MonoBehaviour
    {
        public static BattleController instance;

        public Battle Battle { get { return _battle; } set { _battle = value; } }

        [SerializeField]
        private Battle _battle;

        private void Awake()
        {
            instance = this;
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }
}