using Cysharp.Threading.Tasks;
using Dyra.Flow;
using UIFramework.Runtime;
using VContainer;

namespace Dyra
{
    public class ResultState : FSMState
    {
        [Inject] private UIFrame _uiFrame;
    
        public override UniTask OnEnter()
        {
            _uiFrame.Open<ResultScreen>();    
            return base.OnEnter();
        }

        public override UniTask OnExit()
        {
            _uiFrame.Close<ResultScreen>();
            return base.OnExit();
        }
    }
}