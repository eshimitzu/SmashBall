using Dyra;
using Dyra.Flow;
using UnityEngine;
using VContainer;
using VContainer.Unity;

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
