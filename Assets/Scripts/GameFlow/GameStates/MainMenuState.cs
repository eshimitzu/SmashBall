using Cysharp.Threading.Tasks;
using Dyra.Flow;
using SmashBall.UI.Screens;
using UIFramework.Runtime;
using VContainer;

namespace SmashBall.GameFlow.GameStates
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