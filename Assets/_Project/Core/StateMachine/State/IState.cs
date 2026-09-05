using System.Collections;

namespace TD.Core.StateMachine.State
{
    public interface IState
    {
        public IEnumerator Enter(StateTransition transition);
        public void Tick(float deltaTime);
        public void FixedTick(float fixedDeltaTime);
        public IEnumerator Exit();
    }
}
