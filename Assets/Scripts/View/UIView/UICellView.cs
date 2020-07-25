using UnityEngine;
using UnityEngine.UI;

namespace MyTactial.View.UIView
{
    [RequireComponent(typeof(Image))]
    public class UICellView : MonoBehaviour
    {
        public Model.Cell Cell;
    }
}