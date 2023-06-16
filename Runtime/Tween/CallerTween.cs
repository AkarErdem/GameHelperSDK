using UnityEngine;

namespace GameHelperSDK
{
    public class CallerTween : CallerBase
    {
        [SerializeField] 
        private GenericTween _genericTween;
        
        public override void DoAction()
        {
            if (_genericTween.TargetList.Count == 0) 
                _genericTween.TargetList.Add(this.transform);
            _genericTween.DoAction();
        }
    }
}
