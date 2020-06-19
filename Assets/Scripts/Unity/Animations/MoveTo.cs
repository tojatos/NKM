using Unity.Animations.Parts;
using UnityEngine;

namespace Unity.Animations
{
    public class MoveTo : NkmAnimation
    {
        public MoveTo(Transform trans, Vector3 endPos, float timeToMove)
        {
            AnimationParts.Enqueue(new MoveToPosition(trans, endPos, timeToMove));
        }
        
    }
}