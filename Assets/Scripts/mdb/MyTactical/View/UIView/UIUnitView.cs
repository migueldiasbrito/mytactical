using UnityEngine;
using UnityEngine.UI;

using mdb.MyTactial.Model;

namespace mdb.MyTactial.View.UIView
{
    [RequireComponent(typeof(Image))]
    public class UIUnitView : MonoBehaviour
    {
        public Unit Unit { get { return _unit; } set { _unit = value; } }

        [SerializeField]
        private Unit _unit;
    }
}