using System.Collections;
using Animations.Parts;
using UnityEngine;

namespace Animations
{
    public class TakeDamage : NkmAnimation
    {
        private const float TimeToMove = 0.08f;
        
        public TakeDamage(Transform trans, string value, Color color)
        {
            AllowPlayingOtherAnimations = true;
            Vector3 tilt = new Vector3(0.5f, 0, 0.3f);
//            AnimationParts.Enqueue(new Tilt(trans));
            AnimationParts.Enqueue(new MoveToPosition(trans, trans.position + tilt, TimeToMove));
            AnimationParts.Enqueue(new MoveToPosition(trans, trans.position, TimeToMove));
//            AnimationParts.Enqueue(new FloatingInfo(trans, value, color));
        }
        
    }
}