using UnityEngine;
using Zenject;

namespace CurrentGame.Gameplay.Views
{
    public class ScreenAdjuster : MonoBehaviour
    {
        [Inject] private PaletteView paletteView;

        private Camera camera;
        private float cameraSize;
        
        public float ScreenWidth { get; private set; }

        private void Start()
        {
            Adjust();
        }
        
#if UNITY_EDITOR
        private void Update()
        {
            Adjust();
        }
#endif        

        private void Adjust()
        {
            if (camera == null)
            {
                camera = Camera.main;
                if (camera != null) cameraSize = camera.orthographicSize;
            }
            
            var screenRatio = (float)Screen.width / Screen.height;
            var targetRatio = 9f / 16f;
            
            if (screenRatio >= targetRatio)
            {
                camera.orthographicSize = cameraSize;
            }
            else
            {
                camera.orthographicSize = cameraSize * (targetRatio / screenRatio);
            }
            
            ScreenWidth = camera.orthographicSize * 2f * screenRatio;
            
            var safeArea = Screen.safeArea;
            var safeAreaBottom = (safeArea.yMin - Screen.height) / Screen.height * camera.orthographicSize * 2f;
            paletteView.transform.position = new Vector3(0f, camera.orthographicSize + LevelView.PALETTE_HEIGHT/2f + safeAreaBottom, 0f);
        }
    }
}