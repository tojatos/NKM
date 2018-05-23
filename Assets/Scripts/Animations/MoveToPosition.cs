using System.Collections;
using UnityEngine;

namespace Animations
{
    public class MoveToPosition : NkmAnimation
    {
        private readonly Transform _transformToMove;
        private readonly Vector3 _endPos;
        private readonly float _timeToMove;
	    private readonly bool _allowPlayingOtherAnimations;
        
        public MoveToPosition(Transform trans, Vector3 endPos, float timeToMove, bool allowPlayingOtherAnimaitions = true)
        {
            _transformToMove = trans;
            _endPos = endPos;
            _timeToMove = timeToMove;
		    _allowPlayingOtherAnimations = allowPlayingOtherAnimaitions;
        }
        public override IEnumerator Play()
        {
	        if (_allowPlayingOtherAnimations) AnimationPlayer.CanPlayNext = true;
            var t = 0f;
            Vector3 currentPos = _transformToMove.position;
            while (t < 1)
            {
                t += Time.deltaTime / _timeToMove;
                _transformToMove.position = Vector3.Lerp(currentPos, _endPos, t);
                yield return null;
            }

            AnimationPlayer.CanPlayNext = true;

        }
    }
}