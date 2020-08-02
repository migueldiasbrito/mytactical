using UnityEngine;
using UnityEngine.UI;

using mdb.MyTactial.Controller;
using mdb.MyTactial.Model;

namespace mdb.MyTactial.View.UIView
{
    [RequireComponent(typeof(Image))]
    public class UICellView : MonoBehaviour
    {
        public int CellIndex;

        private Cell _cell;

        private Image _image;
        private Color _defaultColor;

        private void Start()
        {
            _cell = BattleController.instance.Battle.Cells[CellIndex];
            _cell.StateChangedCallback += CellStateChanged;

            _image = GetComponent<Image>();
            _defaultColor = _image.color;
        }

        private void CellStateChanged(bool active)
        {
            _image.color = active ? Color.black : _defaultColor;
        }
    }
}