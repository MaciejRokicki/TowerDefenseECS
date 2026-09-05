using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Scripting.LifecycleManagement;
using UnityEngine;

namespace TD.Core.StateMachine.State
{
    public partial class StateMachine : MonoBehaviour
    {
        [AutoStaticsCleanup]
        public static StateMachine Instance { get; private set; }

        private Dictionary<Type, IState> states;

        public IState CurrentState { get; private set; }
        public bool IsTransitioning { get; private set; }

        private void Awake()
        {
            Instance = this;

            states = new Dictionary<Type, IState>();
        }

        private void Update()
        {
            if (IsTransitioning)
                return;

            if (CurrentState == null)
                return;

            CurrentState.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
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

        public void ChangeState<T>(object payload = null) where T : class, IState
        {
            if (IsTransitioning)
            {
                Debug.LogWarning("Can't change state during transitioing.");
                return;
            }

            if (!states.TryGetValue(typeof(T), out var nextState))
            {
                throw new InvalidOperationException(string.Concat("State: ", typeof(T).Name, " not found."));
            }

            StartCoroutine(ChangeState(nextState, payload));
        }

        private IEnumerator ChangeState(IState nextState, object payload)
        {
            IsTransitioning = true;
            if (CurrentState != null)
                yield return CurrentState.Exit();

            CurrentState = nextState;
            yield return CurrentState.Enter(new StateTransition(payload));
            IsTransitioning = false;
        }
    }
}