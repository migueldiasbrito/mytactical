namespace mdb.StateMachine
{
    public class State
    {
        public delegate void BasicDelegate();
        public event BasicDelegate OnEnter;
        public event BasicDelegate OnExit;

        public delegate void OnClickDelegate(object obj);
        public event OnClickDelegate OnClickEvent;

        public void OnStateEnter()
        {
            OnEnter?.Invoke();
        }

        public void OnStateExit()
        {
            OnExit?.Invoke();
        }

        public void OnClick(object obj)
        {
            OnClickEvent?.Invoke(obj);
        }
    }
}