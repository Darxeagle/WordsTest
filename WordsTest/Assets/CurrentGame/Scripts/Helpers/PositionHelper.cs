using UnityEngine;

namespace CurrentGame.Helpers
{
    public static class PositionHelper
    {
        private static Camera mainCamera;
        
        public static Vector3 ScreenToWorld(Vector3 screenPosition)
        {
            mainCamera ??= Camera.main;
            screenPosition.z = -mainCamera.transform.position.z;
            return mainCamera.ScreenToWorldPoint(screenPosition);
        }

        public static Rect GetScreenBounds()
        {
            mainCamera ??= Camera.main;
            
            float distance = Mathf.Abs(mainCamera.transform.position.z);
            
            Vector3 bottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, distance));
            Vector3 topRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, distance));
            
            Rect screenBounds = new Rect(
                bottomLeft.x,
                bottomLeft.y,
                topRight.x - bottomLeft.x,
                topRight.y - bottomLeft.y
            );
            
            return screenBounds;
        }
    }
}