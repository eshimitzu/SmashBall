using SmashBall.Configs;
using SmashBall.UI.Screens;
using UIFramework.Runtime;
using UnityEngine;
using VContainer;

namespace SmashBall.Gameplay
{
    public class PlayerInputController : MonoBehaviour
    {
        [SerializeField] private Player player;

        [Inject] private PlayerInput playerInput;
        [Inject] private GameplayConfig gameplayConfig;
        [Inject] private IGameplay gameplay;
        [Inject] private UIFrame uiFrame;

        private void Update()
        {
            if (gameplay.CurrentGameplayState.Value == GameplayState.Play)
            {
                if (playerInput.InputVector.magnitude > 0)
                {
                    Vector3 dir = new Vector3(playerInput.InputVector.x, 0, playerInput.InputVector.y);
                    Vector3 delta = dir * (gameplayConfig.playerSpeed * Time.deltaTime);
                    player.Move(delta + Physics.gravity * Time.deltaTime);

                    var lookRotation = Quaternion.LookRotation(dir, Vector3.up);
                    transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, gameplayConfig.playerRotationSpeed * Time.deltaTime);
                }

                if (playerInput.IsPointerUp)
                {
                    player.Attack();
                    playerInput.IsPointerUp = false;
                }
        
                player.AnimationsController.UpdateSpeed(playerInput.InputVector.magnitude);
            }
            else if (gameplay.CurrentGameplayState.Value == GameplayState.WaitForServe)
            {
                if (playerInput.IsPointerUp)
                {
                    var screen = uiFrame.GetScreen<BattleScreen>();
                    HitQuality power = (HitQuality)(screen.ServeBar.Power * 3);
                    player.Serve(power);
                
                    playerInput.IsPointerUp = false;
                }
            }
        }
    }
}
