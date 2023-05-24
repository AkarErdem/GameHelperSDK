using System.Collections.Generic;
using UnityEngine;

namespace GameHelperSDK
{
    public static class PerformanceHelpers
    {
        private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new();

        public static WaitForSeconds GetWait(float time)
        {
            if (WaitDictionary.TryGetValue(time, out var wait)) return wait;
            
            WaitDictionary[time] = new WaitForSeconds(time);
            return WaitDictionary[time];
        }
    }
}