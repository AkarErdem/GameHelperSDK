using System;
using UnityEngine;

namespace GameHelperSDK
{
    public class Camera2DFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _damping = 1;
        [SerializeField] private float _lookAheadFactor = 3;
        [SerializeField] private float _lookAheadReturnSpeed = 0.5f;
        [SerializeField] private float _lookAheadMoveThreshold = 0.1f;

        private float _offsetZ;
        private Vector3 _lastTargetPosition;
        private Vector3 _currentVelocity;
        private Vector3 _lookAheadPos;

        private void Start()
        {
            _lastTargetPosition = _target.position;
            _offsetZ = (transform.position - _target.position).z;
            transform.parent = null;
        }

        private void Update()
        {
            if (_target == null) return;
            
            // Only update lookahead pos if accelerating or changed direction
            float xMoveDelta = (_target.position - _lastTargetPosition).x;

            bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > _lookAheadMoveThreshold;

            if (updateLookAheadTarget)
            {
                _lookAheadPos = _lookAheadFactor*Vector3.right*Mathf.Sign(xMoveDelta);
            }
            else
            {
                _lookAheadPos = Vector3.MoveTowards(_lookAheadPos, Vector3.zero, Time.deltaTime*_lookAheadReturnSpeed);
            }

            Vector3 aheadTargetPos = _target.position + _lookAheadPos + Vector3.forward * _offsetZ;
            Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref _currentVelocity, _damping);

            transform.position = newPos;

            _lastTargetPosition = _target.position;
        }
    }
}
