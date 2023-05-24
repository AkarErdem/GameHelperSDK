using UnityEngine;

namespace GameHelperSDK
{
    public static class CameraHelpers
    {
        private static Camera _mainCamera;
        public static Camera MainCamera
        {
            get
            {
                // Camera.main is not necessary expensive anymore,
                // but still it's an easy access.
                if (_mainCamera == null)
                    _mainCamera = Camera.main;
                return _mainCamera;
            }
        }
    }
}