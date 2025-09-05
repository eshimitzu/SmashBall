using UnityEngine;

namespace SmashBall.Utils
{
    public class GameTimer
    {
        private float startTime;
        private float duration;
        private bool isRunning;

        public bool IsFinished => isRunning && Time.realtimeSinceStartup - startTime >= duration;

        public GameTimer(float seconds)
        {
            Start(seconds);
        }
        
        public void Start(float seconds)
        {
            duration = seconds;
            startTime = Time.realtimeSinceStartup;
            isRunning = true;
        }

        public void Reset()
        {
            isRunning = false;
            duration = 0f;
            startTime = 0f;
        }
    }

}
