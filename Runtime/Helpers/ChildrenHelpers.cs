using UnityEngine;

namespace GameHelperSDK
{
    public static class ChildrenHelpers
    {
        public static void DeleteChildren(this Transform t)
        {
            foreach (Transform child in t) Object.Destroy(child.gameObject);
        }
    }
}