using System;
using UnityEngine;

namespace GameHelperSDK
{
    [Flags]
    public enum UnityActiveEventCallType
    {
        None = 0,
        Awake = 1 << 1,
        OnEnable = 1 << 2,
        Start = 1 << 3,
        Update = 1 << 4,
        FixedUpdate = 1 << 5,
    }
    
    public abstract class CallerBase : MonoBehaviour
    {
        [SerializeField] 
        protected UnityActiveEventCallType _callType;
        
        protected virtual void Awake()
        {
            if (!_callType.HasFlag(UnityActiveEventCallType.Awake)) return;
            DoAction();
        }
        
        protected virtual void OnEnable()
        {
            if (!_callType.HasFlag(UnityActiveEventCallType.OnEnable)) return;
            DoAction();
        }
        
        protected virtual void Start()
        {
            if (!_callType.HasFlag(UnityActiveEventCallType.Start)) return;
            DoAction();
        }

        protected virtual void Update()
        {
            if (!_callType.HasFlag(UnityActiveEventCallType.Update)) return;
            DoAction();
        }
        
        protected virtual void FixedUpdate()
        {
            if (!_callType.HasFlag(UnityActiveEventCallType.FixedUpdate)) return;
            DoAction();
        }
        
        public abstract void DoAction();
        
        public void DestroyObject() => UnityEngine.Object.Destroy(this.gameObject);

    }
}