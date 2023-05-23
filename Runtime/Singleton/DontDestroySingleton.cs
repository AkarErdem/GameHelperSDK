using UnityEngine;

namespace GameHelperSDK
{
    public abstract class DontDestroySingleton<T> : Singleton<T> where T : Component
    {
        protected override void InitializeSingleton()
        {
            base.InitializeSingleton();
            DontDestroyOnLoad(gameObject);
        }
    }
}

