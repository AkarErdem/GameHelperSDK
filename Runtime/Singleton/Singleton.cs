using UnityEngine;

namespace GameHelperSDK
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        public static T Instance;
        
        protected virtual void Awake()
        {
            if (Instance == null)
            {
                InitializeSingleton();
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void InitializeSingleton()
        {
            Instance = this as T;
        }
    }
}


