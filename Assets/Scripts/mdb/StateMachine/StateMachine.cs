namespace mdb.StateMachine
{
    public class StateMachine
    {
        public State CurrentState { get; private set; }

        protected Transition[] _transitions;

        public bool MakeTransition(int transitionIndex)
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
    }
}