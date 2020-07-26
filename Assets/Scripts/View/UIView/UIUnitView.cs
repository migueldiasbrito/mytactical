using UnityEngine;
using UnityEngine.UI;

using MyTactial.Model;

namespace MyTactial.View.UIView
{
    [RequireComponent(typeof(Image))]
    public class UIUnitView : MonoBehaviour
    {
        public Unit Unit { get { return _unit; } set { _unit = value; } }

        [SerializeField]
        private Unit _unit;
    }
}