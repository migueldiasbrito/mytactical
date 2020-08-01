using UnityEngine;
using UnityEngine.UI;

using mdb.MyTactial.Model;

namespace mdb.MyTactial.View.UIView
{
    [RequireComponent(typeof(Image))]
    public class UICellView : MonoBehaviour
    {
        public Cell Cell { get { return _cell; } set { _cell = value; } }

        [SerializeField]
        private Cell _cell;
    }
}