using UnityEngine;

namespace CurrentGame.Gameplay.Views
{
    public class PaletteClusterView : MonoBehaviour
    {
        private const float MIN_SPEED = 0.1f;
        private const float MAX_SPEED = 5f;
        private const float DISTANCE_THRESHOLD = 0.01f;
        
        private ClusterView clusterView;
        private Vector3 targetOffset;
        private Vector3 currentOffset;


        private void Awake()
        {
            if (clusterView == null)
            {
                clusterView = GetComponentInChildren<ClusterView>();
            }
        }

        public ClusterView GetClusterView()
        {
            return clusterView;
        }

        public void SetClusterView(ClusterView view)
        {
            clusterView = view;
            if (view != null)
            {
                view.transform.SetParent(transform, false);
                view.transform.localPosition = Vector3.zero;
            }
        }

        public void SetOffset(Vector3 offset)
        {
            currentOffset = offset;
            if (clusterView != null)
            {
                clusterView.transform.localPosition = offset;
            }
        }

        public void SetTargetOffset(Vector3 offset)
        {
            targetOffset = offset;
        }

        private void Update()
        {
            if (clusterView != null)
            {
                float distance = Vector3.Distance(currentOffset, targetOffset);
                if (distance > DISTANCE_THRESHOLD)
                {
                    // Speed inversely proportional to distance, clamped between MIN_SPEED and MAX_SPEED
                    float speed = Mathf.Lerp(MIN_SPEED, MAX_SPEED, 1f / (1f + distance));
                    currentOffset = Vector3.Lerp(currentOffset, targetOffset, speed * Time.deltaTime);
                    clusterView.transform.localPosition = currentOffset;
                }
            }
        }
    }
}