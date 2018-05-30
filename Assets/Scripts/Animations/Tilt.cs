using Animations.Parts;
using UnityEngine;

namespace Animations
{
    public class Tilt : NkmAnimation
    {
        private const float TimeToMove = 0.08f;
        
        public Tilt(Transform trans)
        {
//            AllowPlayingOtherAnimations = true;
            var tilt = new Vector3(0.5f, 0, 0.3f);
            AnimationParts.Enqueue(new MoveToPosition(trans, trans.position + tilt, TimeToMove));
            AnimationParts.Enqueue(new MoveToPosition(trans, trans.position, TimeToMove));
        }
        
    }
}