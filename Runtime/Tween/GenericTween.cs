using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
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


#if ODIN_INSPECTOR
        [Title("Debug")]
#else
        [Header("Debug")]
#endif
        public bool IsLogEnabled;
        
        // Private
        private GameDebug _logger;
      
        // State
        public bool IsPlaying { get; private set; }

        // public GenericTween(IEnumerable<Transform> targets, IEnumerable<SequenceData> sequences)
        // {
        //     TargetList = new List<Transform>(targets);
        //     SequenceDataList = new List<SequenceData>(sequences);
        // }
        
        public void DoAction()
        {
            // Target list must have at least one item
            if (TargetList.IsNullOrEmpty())
            {
                _logger.LogWarning("Targets list is empty on generic tween");
                return;
            }
            
            // Remove null target items
            TargetList = TargetList.Where(item => item != null).ToList();
            
            // Create logger
            if (_logger == null)
            {
                _logger = new GameDebug($"[GenericTween] ");
            }
            _logger.IsEnabled = IsLogEnabled;
            
            for (var i = 0; i < TargetList.Count; i++)
            {
                var target = TargetList[i];

                var allSequence = _DoActionInternal(target);

                if (i == 0)
                {
                    allSequence.onPlay = AllSequencePlay;
                }
                if (i == TargetList.Count - 1)
                {
                    allSequence.onComplete = AllSequenceComplete;
                }
                allSequence.Play();
            }
        }

        private Sequence _DoActionInternal(Transform target)
        { 
            // Clear playing tweens
            DOTween.Kill(target);
            
            // Create all sequences
            var allSequence = DOTween.Sequence().Pause();
            
            // Assign sequences
            for (var i = 0; i < SequenceDataList.Count; i++)
            {
                var sequenceIndex = i;
                var sequenceData = SequenceDataList[i];
                var sequence = DOTween.Sequence().Pause();

                // Invoke OnSequenceStartEvents when sequence starts
                if (sequenceData.OnSequenceStartEvents)
                {
                    sequence.AppendCallback(() =>
                    {
                        _logger.Log($"Sequence {sequenceIndex} started");
                        sequenceData.OnSequenceStart?.Invoke();
                    });
                }

                // Assign tweens
                foreach (var tweenData in sequenceData.TweenDataList)
                {
                    var tween = GetTween(target, tweenData, sequenceData);

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
                        _logger.Log($"Sequence {sequenceIndex} completed");
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

            return allSequence;
        }

        private void AllSequencePlay()
        {
            _logger.Log("All sequence started");
            IsPlaying = true;
        }
        
        private void AllSequenceComplete()
        {
            _logger.Log("All sequence completed");
            IsPlaying = false;
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
                case TweenActionType.JumpToTarget: 
                        tween = target.DOJump(tweenData.JumpToTarget.position,
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
        [ShowIf("@ActionType == TweenActionType.Jump || ActionType == TweenActionType.JumpToTarget")] 
#endif
        public int NumberOfJumps;
#if ODIN_INSPECTOR
        [ShowIf("@ActionType == TweenActionType.Jump || ActionType == TweenActionType.JumpToTarget")] 
#endif
        public float JumpPower;  
#if ODIN_INSPECTOR
        [ShowIf(nameof(ActionType), TweenActionType.Jump)] 
#endif
        public Vector3 JumpPosition;
#if ODIN_INSPECTOR
        [ShowIf(nameof(ActionType), TweenActionType.JumpToTarget)] 
#endif
        public Transform JumpToTarget;
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
        JumpToTarget = 9,
    }
}
