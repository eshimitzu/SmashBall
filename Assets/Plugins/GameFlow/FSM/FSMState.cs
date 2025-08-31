using System;
using Cysharp.Threading.Tasks;

namespace Dyra.Flow
{
    public abstract class FSMState : IDisposable
    {
        public void SetDependencies(GameFSM fsm)
        {
            _parentFSM = fsm;
        }

        protected GameFSM _parentFSM;
        public virtual UniTask OnEnter()=>UniTask.CompletedTask;
        public virtual UniTask OnExit() => UniTask.CompletedTask;

        public virtual void Dispose(){}
    }
}
