using Unity.Animations.Parts;
using UnityEngine;

namespace Unity.Animations
{
    public class MoveTo : NkmAnimation
    {
        private readonly Transform _transformToMove;
        private readonly Vector3 _endPos;
        private readonly float _timeToMove;
        private readonly bool _allowPlayingOtherAnimations;
        
        public MoveTo(Transform trans, Vector3 endPos, float timeToMove)
        {
            AnimationParts.Enqueue(new MoveToPosition(trans, endPos, timeToMove));
        }
        
    }
}