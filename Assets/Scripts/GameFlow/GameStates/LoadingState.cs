using Cysharp.Threading.Tasks;
using Dyra;
using Dyra.Flow;
using SmashBall.Loading;
using SmashBall.UI.Screens;
using UIFramework.Runtime;
using VContainer;

namespace SmashBall.GameFlow.GameStates
{
    public class LoadingState : FSMState
    {
        [Inject] private UIFrame _uiFrame;
        [Inject] private IGameLoader _gameloader;

        public override async UniTask OnEnter()
        {
            _uiFrame.Open<LoadingScreen>();
            await _gameloader.Load();
            _parentFSM.GoTo<MainMenuState>();
        }
        
        public override UniTask OnExit()
        {
            _uiFrame.Close<LoadingScreen>();
            return base.OnExit();
        }
    }
}
