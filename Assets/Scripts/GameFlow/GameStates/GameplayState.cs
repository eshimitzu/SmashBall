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
    }
}