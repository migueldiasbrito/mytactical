namespace mdb.StateMachine
{
    public class StateMachine
    {
        public State CurrentState { get; private set; }

        Transition[] _transitions;

        public StateMachine(Transition[] transitions)
        {
            _transitions = transitions;
        }

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