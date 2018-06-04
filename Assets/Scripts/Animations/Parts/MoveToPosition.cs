using System.Collections;
using UnityEngine;

namespace Animations.Parts
{
    public class MoveToPosition : NkmAnimationPart
    {
        private readonly Transform _transformToMove;
        private readonly Transform _targetTransform;
        private readonly bool _useReference;
        private readonly Vector3 _refPos;
        
        private Vector3 _endPos;
        private readonly float _timeToMove;
        
        public MoveToPosition(Transform trans, Vector3 pos, float timeToMove, bool useReference = false)
        {
            _transformToMove = trans;
            if (useReference) _refPos = pos; else _endPos = pos;
            _timeToMove = timeToMove;
            _useReference = useReference;
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
            if (_useReference) _endPos = _transformToMove.position + _refPos;
            while (t < 1)
            {
                t += Time.deltaTime / _timeToMove;
                if (_targetTransform != null) _endPos = _targetTransform.position;
//                else if (_useReference) _endPos = _transformToMove.position + _refPos;
                _transformToMove.position = Vector3.Lerp(currentPos, _endPos, t);
                yield return null;
            }

            IsFinished = true;

        }
    }
}
