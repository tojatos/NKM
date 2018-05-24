using System.Collections;
using UnityEngine;

namespace Animations.Parts
{
    public class MoveToPosition : NkmAnimationPart
    {
        private readonly Transform _transformToMove;
        private readonly Transform _targetTransform;
        
        private Vector3 _endPos;
        private readonly float _timeToMove;
	    private readonly bool _allowPlayingOtherAnimations;
        
        public MoveToPosition(Transform trans, Vector3 endPos, float timeToMove)
        {
            _transformToMove = trans;
            _endPos = endPos;
            _timeToMove = timeToMove;
        }
        
        public MoveToPosition(Transform trans, Transform targetTransform, float timeToMove)
        {
            _transformToMove = trans;
            _targetTransform = targetTransform;
            _timeToMove = timeToMove;
        }
        public override IEnumerator Play()
        {
            var t = 0f;
            Vector3 currentPos = _transformToMove.position;
            while (t < 1)
            {
                t += Time.deltaTime / _timeToMove;
                if (_targetTransform != null) _endPos = _targetTransform.position;
                _transformToMove.position = Vector3.Lerp(currentPos, _endPos, t);
                yield return null;
            }

            IsFinished = true;

        }
    }
}
