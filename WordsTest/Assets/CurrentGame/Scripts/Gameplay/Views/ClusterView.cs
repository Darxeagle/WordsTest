using System;
using CurrentGame.Gameplay.Models;
using CurrentGame.Helpers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CurrentGame.Gameplay.Views
{
    public class ClusterView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private Transform container;
        [SerializeField] private Transform lettersContainer;
        [SerializeField] private LetterView letterPrefab;
        [SerializeField] private SpriteRenderer frameRenderer;
        [SerializeField] private BoxCollider2D collider;
        
        public event Action<ClusterView, Cluster, Vector3> OnDragBegin;
        public event Action<ClusterView, Cluster, Vector3> OnDragProgress;
        public event Action<ClusterView, Cluster, Vector3> OnDragEnd;
        
        private Cluster model;
        private Vector3 dragOffset;
        private Vector3 startPosition;
        private bool isDragging;
        private LetterView[] letterViews;
        
        public Cluster Model => model;
        

        public void Initialize(Cluster clusterData)
        {
            model = clusterData;
            CreateLetters();
        }

        private void CreateLetters()
        {
            foreach (Transform child in lettersContainer)
            {
                Destroy(child.gameObject);
            }
            
            letterViews = new LetterView[model.length];
            
            for (int i = 0; i < model.length; i++)
            {
                LetterView letter = Instantiate(letterPrefab, lettersContainer);
                letter.transform.localPosition = new Vector3((i * LevelView.LETTER_DISTANCE), 0f, 0f);

                letterViews[i] = letter;
                if (letterViews[i] != null)
                {
                    letterViews[i].SetLetter(model.characters[i], model.color);
                }
            }
            
            var width = GetWidth();
            frameRenderer.size = new Vector2(width + LevelView.FRAME_PADDING*2f, LevelView.LETTER_SIZE + LevelView.FRAME_PADDING*2f);
            frameRenderer.transform.localPosition = new Vector3(width / 2f - LevelView.LETTER_SIZE / 2f, 0f, frameRenderer.transform.localPosition.z);
            collider.size = frameRenderer.size;
            collider.offset = frameRenderer.transform.localPosition;
        }
        
        public float GetWidth()
        {
            return (model.length-1) * LevelView.LETTER_DISTANCE + LevelView.LETTER_SIZE;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isDragging = true;
            startPosition = transform.position;
            Vector3 mousePosition = PositionHelper.ScreenToWorld(eventData.position);
            dragOffset = transform.position - mousePosition;
            container.DOScale(1.1f, 0.2f);
            container.localPosition = new Vector3(0f, 0f, -0.1f);
            OnDragBegin?.Invoke(this, model, transform.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging) return;
            transform.position = PositionHelper.ScreenToWorld(eventData.position) + dragOffset;
            OnDragProgress?.Invoke(this, model, transform.position);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isDragging) return;
            isDragging = false;
            container.DOScale(1f, 0.2f);
            container.localPosition = new Vector3(0f, 0f, 0f);
            OnDragEnd?.Invoke(this, model, transform.position);
        }

        public int GetCharacterCount()
        {
            return model?.length ?? 0;
        }
    }
}