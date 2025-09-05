using Dyra;
using Dyra.Flow;
using SmashBall.GameFlow.GameStates;
using UIFramework;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace SmashBall.UI.Screens
{
    public class MainMenuScreen : UIScreen
    {
        [SerializeField] private Button _playButton;

        [Inject] private GameFSM _gameFsm;

        private void Awake()
        {
            _playButton.onClick.AddListener(OnPlayButtonClicked);
        }

        private void OnPlayButtonClicked()
        {
            _gameFsm.GoTo<GameplayState>();
        }
    }
}