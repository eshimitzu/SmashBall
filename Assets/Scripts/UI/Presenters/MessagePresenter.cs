using SmashBall.Gameplay;
using SmashBall.UI.Components;
using SmashBall.UI.Screens;
using UIFramework.Runtime;
using UnityEngine;
using VContainer;

namespace SmashBall.UI.Presenters
{
    public class MessagePresenter
    {
        [Inject] private UIFrame frame;
        [Inject] private GameplayCamera camera;


        public void ShowDamage(int damage, Transform target, Vector3 offset)
        {
            var battleScreen = frame.GetScreen<BattleScreen>();
            battleScreen.ShowDamage(damage, target, offset, camera.Cam);
        }

        public void ShowHitQuality(HitQuality quality, Transform target, Vector3 offset)
        {
            var battleScreen = frame.GetScreen<BattleScreen>();
            battleScreen.ShowHitQuality(quality, target, offset, camera.Cam);
        }
    }
}
