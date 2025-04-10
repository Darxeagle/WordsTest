using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace CurrentGame.Sounds
{
    [CreateAssetMenu(fileName = "SoundsConfig", menuName = "ScriptableObjects/SoundsConfig", order = 1)]
    public class SoundsConfig : ScriptableObject, IInitializable
    {
        [SerializeField] private List<SoundConfigEntry> soundsConfigEntries;
        
        private Dictionary<SoundId, AudioClip> soundsDictionary;
        
        public void Initialize()
        {
            soundsDictionary = soundsConfigEntries.ToDictionary(entry => entry.soundId, entry => entry.audioClip);
        }
        
        public AudioClip GetSound(SoundId soundId)
        {
            if (soundsDictionary.TryGetValue(soundId, out var audioClip))
            {
                return audioClip;
            }
            else
            {
                Debug.LogError($"Sound with ID {soundId} not found.");
                return null;
            }
        }
    }
}