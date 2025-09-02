using Dyra.Flow;
using UIFramework;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Dyra
{
    public class MainMenuScreen : UIScreen
    {
        [SerializeField] private Button _playButton;

        [Inject] private GameFSM _gameFsm;
    
        protected override void OnOpening()
        {
            _playButton.onClick.AddListener(OnPlayButtonClicked);
        }

        private void OnPlayButtonClicked()
        {
            _gameFsm.GoTo<GameplayState>();
        }
    }
}