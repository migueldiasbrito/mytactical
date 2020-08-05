using UnityEngine;

using mdb.MyTactial.Model;

namespace mdb.MyTactial.Controller.BasicAI.Conditions
{
    public class Condition : MonoBehaviour
    {
        public Unit Unit { get; set; }

        public bool TestCondition { get; protected set; }
        public object[] TestResults { get; protected set; }
    }
}