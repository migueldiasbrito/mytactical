namespace mdb.StateMachine
{
    public class State
    {
        public delegate void BaseAction();
        public event BaseAction OnEnter;
        public event BaseAction OnExit;

        public void OnStateEnter()
        {
            OnEnter?.Invoke();
        }

        public void OnStateExit()
        {
            OnExit?.Invoke();
        }
    }
}