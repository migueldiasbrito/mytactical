using System.Collections.Generic;
using UnityEngine;

namespace mdb.StateMachine
{
    public class StateMachine : MonoBehaviour
    {
        public State CurrentState { get; private set; }

        protected Transition[] _transitions;

        private Queue<int> queuedTransitions = new Queue<int>();

        public void AddTransition(int transitionIndex)
        {
            queuedTransitions.Enqueue(transitionIndex);
        }

        public void OnClick(object obj)
        {
            CurrentState.OnClick(obj);
        }

        private bool MakeTransition(int transitionIndex)
        {
            if (transitionIndex < 0 || transitionIndex >= _transitions.Length)
            {
                return false;
            }

            if (CurrentState != _transitions[transitionIndex].From)
            {
                return false;
            }

            _transitions[transitionIndex].From?.OnStateExit();

            CurrentState = _transitions[transitionIndex].To;
            _transitions[transitionIndex].To.OnStateEnter();
            return true;
        }

        private void Update()
        {
            if(queuedTransitions.Count > 0)
            {
                MakeTransition(queuedTransitions.Dequeue());
            }
        }
    }
}