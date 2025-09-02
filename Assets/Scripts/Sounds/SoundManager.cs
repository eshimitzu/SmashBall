using UnityEngine;

namespace Everyday.Sounds
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private SoundBank soundBank;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void PlaySFX(string name)
        {
            var clip = soundBank.GetClip(name);
            if (clip != null)
                sfxSource.PlayOneShot(clip);
            else
                Debug.LogWarning($"[SoundManager] SFX '{name}' not found.");
        }

        public void PlayMusic(string name, bool loop = true)
        {
            var clip = soundBank.GetClip(name);
            if (clip != null)
            {
                musicSource.clip = clip;
                musicSource.loop = loop;
                musicSource.Play();
            }
            else
            {
                Debug.LogWarning($"[SoundManager] Music '{name}' not found.");
            }
        }

        public void StopMusic() => musicSource.Stop();
    }
}