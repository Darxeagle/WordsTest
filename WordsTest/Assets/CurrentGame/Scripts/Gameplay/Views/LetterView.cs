using DG.Tweening;
using TMPro;
using UnityEngine;

namespace CurrentGame.Gameplay.Views
{
    public class LetterView : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textMesh;
        [SerializeField] private SpriteRenderer backgroundRenderer;
        
        public void SetLetter(char letter)
        {
            if (textMesh != null)
            {
                textMesh.text = letter.ToString();
            }
        }

        public void SetHighlight(bool isValid)
        {
            if (backgroundRenderer != null)
            {
                Color color = isValid ? Color.green : Color.red;
                color.a = 0.5f;
                backgroundRenderer.DOColor(color, 0.2f);
            }
        }

        public void ResetHighlight()
        {
            if (backgroundRenderer != null)
            {
                backgroundRenderer.DOColor(Color.white, 0.2f);
            }
        }
    }
}