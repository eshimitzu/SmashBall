using Cysharp.Threading.Tasks;
using UnityEngine;

public class MockGameLoader : IGameLoader
{
    private float duration = 2f;
    public float CurrentProgress { get; protected set; }
    
    
    public async UniTask Load()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / duration);
            
            CurrentProgress = progress;
            
            await UniTask.Yield();
        }

        CurrentProgress = 1f;
    }
}
