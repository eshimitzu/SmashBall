using System.Collections.Generic;
using UIFramework.Runtime;

namespace Dyra.Flow
{
    public class InjectableGameFSM : GameFSM
    {
        public InjectableGameFSM(IReadOnlyList<FSMState> states)
        {
            foreach (var fsmState in states)
            {
                fsmState.SetDependencies(this);
                RegisterState(fsmState);
            }
        }
    }
}
