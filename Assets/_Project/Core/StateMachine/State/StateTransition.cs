namespace TD.Core.StateMachine.State
{
    public readonly struct StateTransition
    {
        public readonly object Payload;

        public StateTransition(object payload = null)
        {
            Payload = payload;
        }

        public T GetPayload<T>() => (T)Payload;
    }
}
