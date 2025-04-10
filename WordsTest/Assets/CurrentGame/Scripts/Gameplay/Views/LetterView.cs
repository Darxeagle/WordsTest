using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace CurrentGame.Gameplay.Views
{
    public class LetterView : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textMesh;
        [SerializeField] private SpriteRenderer backgroundRenderer;
        [SerializeField] private List<Sprite> backgrounds;
        
        public void SetLetter(char letter, int color)
        {
            textMesh.text = letter.ToString();
            backgroundRenderer.sprite = backgrounds[color % backgrounds.Count];
        }

        public void SetHighlight(bool isValid)
        {
            Color color = isValid ? Color.green : Color.red;
            color.a = 0.5f;
            backgroundRenderer.DOColor(color, 0.2f);
        }

        public void ResetHighlight()
        {
            backgroundRenderer.DOColor(Color.white, 0.2f);
        }
    }
}