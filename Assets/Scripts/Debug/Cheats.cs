using Dyra;
using Dyra.Flow;
using SmashBall.GameFlow.GameStates;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace SmashBall.Cheats
{
    public class Cheats : ITickable
    {
        [Inject] private GameFSM fsm;


        public void Tick()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                fsm.GoTo<ResultState>();
            }
        }
    }
}
