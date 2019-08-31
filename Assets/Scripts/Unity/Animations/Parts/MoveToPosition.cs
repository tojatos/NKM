using System.Collections;
using UnityEngine;

namespace Unity.Animations.Parts
{
    public class MoveToPosition : NkmAnimationPart
    {
        private readonly Transform _transformToMove;
        private readonly Transform _targetTransform;
        private readonly bool _useReference;
        private readonly bool _local;
        private readonly Vector3 _refPos;

        private Vector3 _endPos;
        private readonly float _timeToMove;

        public MoveToPosition(Transform trans, Vector3 pos, float timeToMove, bool useReference = false, bool local = false)
        {
            _transformToMove = trans;
            if (useReference) _refPos = pos; else _endPos = pos;
            _timeToMove = timeToMove;
            _useReference = useReference;
            _local = local;
        }

        public MoveToPosition(Transform trans, Transform targetTransform, float timeToMove)
        {
            _transformToMove = trans;
            _targetTransform = targetTransform;
            _timeToMove = timeToMove;
        }
        public override IEnumerator Play()
        {
            if (_transformToMove == null)
            {
                IsFinished = true;
                yield break;
            }
            float t = 0f;
            Vector3 GetCurrentPos() => _local ? _transformToMove.localPosition : _transformToMove.position;
            Vector3 currentPos = GetCurrentPos();
            if (_useReference) _endPos = GetCurrentPos() + _refPos;
            while (t < 1)
            {
                t += Time.deltaTime / _timeToMove;
                if (_targetTransform != null) _endPos = _targetTransform.position;
                if (_local)
                    _transformToMove.localPosition = Vector3.Lerp(currentPos, _endPos, t);
                else
                    _transformToMove.position = Vector3.Lerp(currentPos, _endPos, t);
                yield return null;
            }

            IsFinished = true;

        }
    }
}
