using System.Collections.Generic;
using DG.Tweening;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace GameHelperSDK
{
    [System.Serializable]
    public class GenericTween
    {
#if ODIN_INSPECTOR
        [Title("Targets")]
#else
        [Header("Targets")]
#endif
        public List<Transform> TargetList;
            
#if ODIN_INSPECTOR
        [Title("Sequences")]
#else
        [Header("Sequences")]
#endif
        [SerializeField] 
        public List<SequenceData> SequenceDataList;

#if ODIN_INSPECTOR
        [Title("Loops")]
#else
        [Header("Loops")]
#endif
        [SerializeField] 
        public bool HasLoops;
        
#if ODIN_INSPECTOR
        [ShowIf("@HasLoops")] 
#endif
        public LoopType LoopingType;
        
#if ODIN_INSPECTOR
        [ShowIf("@HasLoops")] 
#endif
        public bool InfiniteLoop;
        
#if ODIN_INSPECTOR
        [ShowIf("@HasLoops && !InfiniteLoop")] 
#endif
        public uint LoopCount;

        private readonly GameDebug _logger;

        public GenericTween(IEnumerable<Transform> targets, IEnumerable<SequenceData> sequences)
        {
            _logger = new GameDebug($"[GenericTween] ");
            TargetList = new List<Transform>(targets);
            SequenceDataList = new List<SequenceData>(sequences);
        }
        
        public void DoAction()
        {
            if (TargetList.IsNullOrEmpty())
            {
                _logger.LogWarning("Targets list is empty on generic tween");
                return;
            }
            
            foreach (var target in TargetList)
            {
                _DoActionInternal(target);
            }
        }                  

        private void _DoActionInternal(Transform _target)
        { 
            // Clear playing tweens
            _target.DOKill();
            DOTween.Kill(_target);
                
            // Create all sequences
            var allSequence = DOTween.Sequence();
            
            // Assign sequences
            foreach (var sequenceData in SequenceDataList)
            {
                var sequence = DOTween.Sequence();
                
                // Invoke OnSequenceStartEvents when sequence starts
                if (sequenceData.OnSequenceStartEvents)
                {
                    sequence.AppendCallback(() =>
                    {
                            //Debug.Log("Start");
                            sequenceData.OnSequenceStart?.Invoke();
                    });
                }
                
                // Assign tweens
                foreach (var tweenData in sequenceData.TweenDataList)
                {
                    var tween = GetTween(_target, tweenData, sequenceData);
                    
                    if (tween != null)
                    {
                            sequence.Join(tween);
                    }
                }
                
                // Invoke OnSequenceCompleteEvents when sequence ends
                if (sequenceData.OnSequenceCompleteEvents)
                {
                    sequence.AppendCallback(() =>
                    {
                            //Debug.Log("Complete");
                            sequenceData.OnSequenceComplete?.Invoke();
                    });
                }
                
                allSequence.Append(sequence);
            }
            
            // Set loops
            if (HasLoops)
            {
                var loopCount = InfiniteLoop ? -1 : (int)LoopCount;
                allSequence.SetLoops(loopCount, LoopingType);
            }
        }
        
        private static Tween GetTween(Transform target, TweenData tweenData, SequenceData sequenceData)
        {
            Tween tween = null;
            var duration = sequenceData.Duration;
            
            switch (tweenData.ActionType)
            {
                case TweenActionType.Move:
                    tween = target.DOMove(tweenData.MoveTo, duration);
                    break;
                case TweenActionType.LocalMove:
                    tween = target.DOLocalMove(tweenData.LocalMoveTo, duration);
                    break;
                case TweenActionType.MoveToTarget:
                    tween = target.DOLocalMove(tweenData.MoveToTarget.position, duration);
                    break;
                case TweenActionType.Scale:
                    tween = target.DOScale(tweenData.ScaleTo, duration);
                    break;
                case TweenActionType.ScaleLikeTarget:
                    tween = target.DOScale(tweenData.ScaleLikeTarget.localScale, duration);
                    break;
                case TweenActionType.Rotate:
                    tween = target.DORotate(tweenData.RotateTo, duration);
                    break;
                case TweenActionType.RotateLikeTarget:
                    tween = target.DORotate(tweenData.RotateLikeTarget.rotation.eulerAngles, duration);
                    break;
                case TweenActionType.Jump: 
                    tween = target.DOJump(tweenData.JumpPosition,
                                          tweenData.JumpPower,
                                          tweenData.NumberOfJumps,
                                          duration);
                        break;
            }
            
            tween.SetEase(tweenData.Ease);
            
            return tween;
        }
    }
    
    [System.Serializable]
    public class SequenceData
    {
#if ODIN_INSPECTOR
        [Title("Sequence Data")]
#else
        [Header("Sequence Data")]
#endif
        public List<TweenData> TweenDataList;
        public float Duration;
        
        public bool OnSequenceStartEvents;
        
#if ODIN_INSPECTOR
        [ShowIf("@OnSequenceStartEvents")] 
#endif
        public UnityEvent OnSequenceStart;
        
        public bool OnSequenceCompleteEvents;
        
#if ODIN_INSPECTOR
        [ShowIf("@OnSequenceCompleteEvents")] 
#endif
        public UnityEvent OnSequenceComplete;
    }
    
    [System.Serializable]
    public class TweenData
    {
#if ODIN_INSPECTOR
        [Title("Tween Data")]
#else
        [Header("Tween Data")]
#endif
        
        public Ease Ease;
        public TweenActionType ActionType;
        
#if ODIN_INSPECTOR
        [ShowIf(nameof(ActionType), TweenActionType.Move)]
#endif
        public Vector3 MoveTo;
        
        
#if ODIN_INSPECTOR
        [ShowIf(nameof(ActionType), TweenActionType.LocalMove)] 
#endif
        public Vector3 LocalMoveTo;
#if ODIN_INSPECTOR
        [ShowIf(nameof(ActionType), TweenActionType.MoveToTarget)] 
#endif
        public Transform MoveToTarget;
#if ODIN_INSPECTOR
        [ShowIf(nameof(ActionType), TweenActionType.Scale)] 
#endif
        public Vector3 ScaleTo;
#if ODIN_INSPECTOR
        [ShowIf(nameof(ActionType), TweenActionType.ScaleLikeTarget)] 
#endif
        public Transform ScaleLikeTarget;
#if ODIN_INSPECTOR
        [ShowIf(nameof(ActionType), TweenActionType.Rotate)] 
#endif
        public Vector3 RotateTo;
#if ODIN_INSPECTOR
        [ShowIf(nameof(ActionType), TweenActionType.RotateLikeTarget)] 
#endif
        public Transform RotateLikeTarget;
        
#if ODIN_INSPECTOR
        [ShowIf(nameof(ActionType), TweenActionType.Jump)] 
#endif
        public int NumberOfJumps;
#if ODIN_INSPECTOR
        [ShowIf(nameof(ActionType), TweenActionType.Jump)] 
#endif
        public Vector3 JumpPosition;
#if ODIN_INSPECTOR
        [ShowIf(nameof(ActionType), TweenActionType.Jump)] 
#endif
        public float JumpPower;        
    }

    public enum TweenActionType
    {
        None = 0,
        Move = 1,
        LocalMove = 2,
        MoveToTarget = 3,
        Scale = 4,
        ScaleLikeTarget = 5,
        Rotate = 6,
        RotateLikeTarget = 7,
        Jump = 8,
    }
}
