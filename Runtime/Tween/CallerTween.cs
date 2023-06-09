using Sirenix.Utilities;
using UnityEngine;

namespace GameHelperSDK
{
    public class CallerTween : CallerBase
    {
        [SerializeField] 
        private GenericTween _genericTween;
        
        public override void DoAction()
        {
            if (_genericTween.TargetList.IsNullOrEmpty()) 
                _genericTween.TargetList.Add(this.transform);
            _genericTween.DoAction();
        }
    }
}
