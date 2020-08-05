using UnityEngine;

using mdb.MyTactial.Model;

namespace mdb.MyTactial.Controller.BasicAI.Actions
{
    public abstract class Action : MonoBehaviour
    {
        public Unit Unit { get; set; }
        public abstract bool DoAction(object[] arguments);
    }
}