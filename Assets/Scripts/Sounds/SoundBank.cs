using UnityEngine;
using System.Collections.Generic;

namespace Dyra.Sounds
{
    [CreateAssetMenu(fileName = "SoundBank", menuName = "Dyra/SoundBank", order = 0)]
    public class SoundBank : ScriptableObject
    {
        public List<SoundEntry> sounds;

        private Dictionary<string, AudioClip> _soundDict;

        public AudioClip GetClip(string name)
        {
            if (_soundDict == null) Init();
            _soundDict.TryGetValue(name, out var clip);
            return clip;
        }

        private void Init()
        {
            _soundDict = new Dictionary<string, AudioClip>();
            foreach (var entry in sounds)
            {
                if (!_soundDict.ContainsKey(entry.name))
                    _soundDict[entry.name] = entry.clip;
            }
        }
    }

    [System.Serializable]
    public class SoundEntry
    {
        public string name;
        public AudioClip clip;
    }
}