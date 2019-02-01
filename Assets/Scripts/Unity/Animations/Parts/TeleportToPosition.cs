using System.Collections;
using UnityEngine;

namespace Unity.Animations.Parts
{
    public class TeleportToPosition : NkmAnimationPart
    {
        private readonly Transform _transformToMove;
        private readonly Transform _targetTransform;
        private readonly Vector3 _targetPosition;
        
        public TeleportToPosition(Transform trans, Vector3 targetPos)
        {
            _transformToMove = trans;
            _targetPosition = targetPos;
        }
        
        public TeleportToPosition(Transform trans, Transform targetTransform)
        {
            _transformToMove = trans;
            _targetTransform = targetTransform;
        }
        public override IEnumerator Play()
        {
            _transformToMove.position = _targetTransform != null ? _targetTransform.position : _targetPosition;

            IsFinished = true;
            yield break;
        }
    }
}
