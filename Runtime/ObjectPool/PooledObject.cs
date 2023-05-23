using UnityEngine;

namespace GameHelperSDK
{
    public class PooledObject : MonoBehaviour
    {
        [Tooltip("Name of the pool to which this object belongs to")]
        [SerializeField] private string _poolName;
        
        [Tooltip("Defines whether the object is waiting in pool or is in use")]
        private bool _isPooled = false;
        
        // Name of the pool that this object belongs to
        public string PoolName
        {
            get => _poolName;
            set => _poolName = value;
        }
        
        // Defines whether the object is waiting in pool or is in use
        public bool IsPooled
        {
            get => _isPooled;
            set => _isPooled = value;
        }

        public void ReturnObjectToPool()
        {
            if (_isPooled) return;
            
            ObjectPool.Instance.ReturnObjectToPool(this);
        }
    }
}
