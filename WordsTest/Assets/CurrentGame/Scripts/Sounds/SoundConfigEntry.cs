using System;
using UnityEngine;

namespace CurrentGame.Sounds
{
    [Serializable]
    public class SoundConfigEntry
    {
        public SoundId soundId;
        public AudioClip audioClip;
    }
}