using UnityEngine;
using UnityEngine.UI;

using MyTactial.Model;

namespace MyTactial.View.UIView
{
    [RequireComponent(typeof(Image))]
    public class UICellView : MonoBehaviour
    {
        public Cell Cell { get { return _cell; } set { _cell = value; } }

        [SerializeField]
        private Cell _cell;
    }
}