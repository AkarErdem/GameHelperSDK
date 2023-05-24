using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameHelperSDK
{
    public static class UIHelpers
    {
        public static bool IsOverUI()
        {
            var results = new List<RaycastResult>();
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            
            return results.Count > 0;
        }

        public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, CameraHelpers.MainCamera, out var result);
            return result;
        }
    }
}

