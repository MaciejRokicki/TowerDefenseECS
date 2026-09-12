using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

namespace TD.Core.StateMachine.State
{
    public sealed class StateMachine : MonoBehaviour, IInitializable, ITickable, IFixedTickable
    {
        private Dictionary<Type, IState> states;

        public IState CurrentState { get; private set; }
        public bool IsTransitioning { get; private set; }

        public void Initialize()
        {
            states = new Dictionary<Type, IState>();
        }

        public void Tick()
        {
            if (IsTransitioning)
                return;

            if (CurrentState == null)
                return;

            CurrentState.Tick(Time.deltaTime);
        }

        public void FixedTick()
        {
            if (IsTransitioning)
                return;

            if (CurrentState == null)
                return;

            CurrentState.FixedTick(Time.fixedDeltaTime);
        }

        public void Register<T>(T state) where T : IState
        {
            states[typeof(T)] = state;
        }

        public bool TryChangeState<T>(object payload = null) where T : class, IState
        {
            if (IsTransitioning)
            {
                Debug.LogWarning("Can't change state during transitioing.");
                return false;
            }

            if (!states.TryGetValue(typeof(T), out var nextState))
            {
                throw new InvalidOperationException(string.Concat("State: ", typeof(T).Name, " not found."));
            }

            IsTransitioning = true;
            StartCoroutine(ChangeState(nextState, payload));
            return true;
        }

        private IEnumerator ChangeState(IState nextState, object payload)
        {
            if (CurrentState != null)
                yield return CurrentState.Exit();

            CurrentState = nextState;
            yield return CurrentState.Enter(new StateTransition(payload));
            IsTransitioning = false;
        }
    }
}