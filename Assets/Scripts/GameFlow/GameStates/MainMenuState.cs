using Cysharp.Threading.Tasks;
using Dyra.Flow;
using UIFramework.Runtime;
using VContainer;

namespace Dyra
{
    public class MainMenuState : FSMState
    {
        [Inject] private UIFrame _uiFrame;
    
        public override UniTask OnEnter()
        {
            _uiFrame.Open<MainMenuScreen>();    
        
            return base.OnEnter();
        }

        public override UniTask OnExit()
        {
            _uiFrame.Close<MainMenuScreen>();
            return base.OnExit();
        }
    }
}