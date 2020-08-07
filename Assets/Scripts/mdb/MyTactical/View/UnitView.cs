using mdb.MyTactial.Model;
using UnityEngine;

namespace mdb.MyTactial.View
{
    public abstract class UnitView : MonoBehaviour
    {
        public abstract void DoPath(Cell[] cells);
    }
}
