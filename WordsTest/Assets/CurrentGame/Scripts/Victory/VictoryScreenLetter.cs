using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace CurrentGame.Victory
{
    public class VictoryScreenLetter : MonoBehaviour
    {
        [SerializeField] private Text letterText;
        
        public void SetLetter(char letter, float delay)
        {
            letterText.text = letter.ToString().ToUpper();
            
            // Initial setup
            transform.localScale = Vector3.zero;
            letterText.color = new Color(1f, 1f, 1f, 0f);
            
            // Animate appearance
            transform.DOScale(1f, 0.3f)
                .SetDelay(delay)
                .SetEase(Ease.OutBack);
                
            letterText.DOFade(1f, 0.2f)
                .SetDelay(delay);
        }
    }
}