using System.Resources;
using CurrentGame.GameFlow;
using UnityEngine;
using Zenject;

namespace CurrentGame.Sounds
{
    public class MusicController : MonoBehaviour, IInitializable
    {
        [SerializeField] private AudioSource audioSource;
        
        [Inject] private GameModel gameModel;
        
        public void Initialize()
        {
            audioSource = GetComponent<AudioSource>();
            SetMusicEnabled(gameModel.MusicEnabled);
        }
        
        public void SetMusicEnabled(bool enabled)
        {
            if (enabled && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
            else if (!enabled && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            gameModel.MusicEnabled = enabled;
        }
    }
}