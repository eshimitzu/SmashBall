using Dyra.Flow;
using UIFramework;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Dyra
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
