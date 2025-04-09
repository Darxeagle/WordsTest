using System;
using CurrentGame.Gameplay.Models;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CurrentGame.Gameplay.Views
{
    public class ClusterView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private Transform lettersContainer;
        [SerializeField] private LetterView letterPrefab;
        [SerializeField] private SpriteRenderer frameRenderer;
        [SerializeField] private BoxCollider2D collider;
        
        private const float FRAME_PADDING = 0.2f;
        
        public event Action<ClusterView, Cluster, Vector3> OnDragBegin;
        public event Action<ClusterView, Cluster, Vector3> OnDragProgress;
        public event Action<ClusterView, Cluster, Vector3> OnDragEnd;
        
        private Cluster model;
        private Vector3 dragOffset;
        private Vector3 startPosition;
        private bool isDragging;
        private LetterView[] letterViews;
        private Camera mainCamera;
        
        public Cluster Model => model;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

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
                    letterViews[i].SetLetter(model.characters[i]);
                }
            }
            
            var width = GetWidth();
            frameRenderer.size = new Vector2(width + FRAME_PADDING*2f, frameRenderer.size.y);
            frameRenderer.transform.localPosition = new Vector3(width / 2f - LevelView.LETTER_SIZE / 2f, 0f);
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
            Vector3 mousePosition = GetPointerWorldPosition(eventData);
            dragOffset = transform.position - mousePosition;
            transform.DOScale(1.1f, 0.2f);
            OnDragBegin?.Invoke(this, model, transform.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging) return;
            transform.position = GetPointerWorldPosition(eventData) + dragOffset;
            OnDragProgress?.Invoke(this, model, transform.position);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isDragging) return;
            isDragging = false;
            transform.DOScale(1f, 0.2f);
            OnDragEnd?.Invoke(this, model, transform.position);
        }

        private Vector3 GetPointerWorldPosition(PointerEventData eventData)
        {
            Vector3 mousePos = eventData.position;
            mousePos.z = -mainCamera.transform.position.z;
            return mainCamera.ScreenToWorldPoint(mousePos);
        }

        public void SetHighlight(bool isValid)
        {
            foreach (var letterView in letterViews)
            {
                if (letterView != null)
                {
                    letterView.SetHighlight(isValid);
                }
            }

            if (frameRenderer != null)
            {
                Color color = isValid ? Color.green : Color.red;
                color.a = 0.5f;
                frameRenderer.DOColor(color, 0.2f);
            }
        }

        public void ResetHighlight()
        {
            foreach (var letterView in letterViews)
            {
                if (letterView != null)
                {
                    letterView.ResetHighlight();
                }
            }

            if (frameRenderer != null)
            {
                frameRenderer.DOColor(Color.white, 0.2f);
            }
        }

        public int GetCharacterCount()
        {
            return model?.length ?? 0;
        }
    }
}