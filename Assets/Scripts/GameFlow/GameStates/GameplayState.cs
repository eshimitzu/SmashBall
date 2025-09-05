using Cysharp.Threading.Tasks;
using Dyra.Flow;
using UIFramework.Runtime;
using UnityEngine.SceneManagement;
using VContainer;

namespace Dyra
{
    public class GameplayState : FSMState
    {
        [Inject] private UIFrame _uiFrame;
    
        public override async UniTask OnEnter()
        {
            _uiFrame.Open<BattleScreen>();    

            await SceneManager.LoadSceneAsync("BattleScene", LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("BattleScene"));
        }
        
        
        public override async UniTask OnExit()
        {
            _uiFrame.Close<BattleScreen>();

            await SceneManager.UnloadSceneAsync("BattleScene");
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("InitialScene"));
        }
    }
}