using Cysharp.Threading.Tasks;
using Dyra.Flow;
using SmashBall.Sounds;
using SmashBall.UI.Screens;
using UIFramework.Runtime;
using UnityEngine.SceneManagement;
using VContainer;

namespace SmashBall.GameFlow.GameStates
{
    public class GameplayState : FSMState
    {
        [Inject] private UIFrame _uiFrame;
        [Inject] private SoundManager soundManager;

        public override async UniTask OnEnter()
        {
            _uiFrame.Open<BattleScreen>();    

            await SceneManager.LoadSceneAsync("BattleScene", LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("BattleScene"));
            
            soundManager.PlayMusic("StadiumAmbient");
        }
        
        
        public override async UniTask OnExit()
        {
            _uiFrame.Close<BattleScreen>();

            await SceneManager.UnloadSceneAsync("BattleScene");
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("InitialScene"));
            
            soundManager.StopMusic();
        }
    }
}