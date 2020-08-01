namespace mdb.StateMachine
{
    public class Transition
    {
        public State From { get; private set; }
        public State To { get; private set; }

        public Transition(State from, State to)
        {
            From = from;
            To = to;
        }
    }
}