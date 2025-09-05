using Dyra;
using Dyra.Flow;
using SmashBall.GameFlow.GameStates;
using UIFramework;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace SmashBall.UI.Screens
{
    public class ResultScreen : UIScreen
    {
        [SerializeField] private Button nextButton;

        [Inject] private GameFSM gameFsm;
        
    
        private void Awake()
        {
            nextButton.onClick.AddListener(OnNextButtonClicked);
        }

        private void OnNextButtonClicked()
        {
            gameFsm.GoTo<MainMenuState>();
        }
    }
}
