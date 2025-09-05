using Dyra;
using UIFramework.Runtime;
using UnityEngine;
using VContainer;

namespace SmashBall.UI.Presenters
{
    public class MessagePresenter
    {
        [Inject] private UIFrame frame;
        [Inject] private GameplayCamera camera;


        public void ShowDamage(int damage, Vector3 pos)
        {
            var battleScreen = frame.GetScreen<BattleScreen>();
            battleScreen.ShowDamage(damage, pos, camera.Cam);
        }
    }
}
