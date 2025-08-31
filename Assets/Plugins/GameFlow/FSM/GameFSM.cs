using System;
using System.Collections.Generic;

namespace Dyra.Flow
{
    public class GameFSM
    {
        private FSMState _currentState;

        private Dictionary<Type, FSMState> _fsmStates = new();

        public FSMState CurrentState => _currentState;

        protected void RegisterState<T>(T state) where T : FSMState
        {
            _fsmStates.Add(state.GetType(), state);
        }

        public void GoTo<T>() where T : FSMState
        {
            if (_currentState != null)
            {
                _currentState.OnExit();
            }

            _currentState = _fsmStates[typeof(T)];

            _currentState.OnEnter();
        }
    }
}
