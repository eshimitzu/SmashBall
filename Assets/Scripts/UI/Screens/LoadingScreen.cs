using Dyra.Flow;
using SmashBall.Loading;
using UIFramework;
using UnityEngine;
using VContainer;

namespace SmashBall.UI.Screens
{
    public class LoadingScreen : UIScreen
    {
        [Inject] private GameFSM _gameFsm;
        [Inject] private IGameLoader _gameloader;
        
        [SerializeField] private RectTransform progressBar;

        
        
        protected override void OnOpening()
        {
        }

        private void Update()
        {
            progressBar.anchorMax = new Vector2(_gameloader.CurrentProgress, progressBar.anchorMax.y);
        }
    }
}