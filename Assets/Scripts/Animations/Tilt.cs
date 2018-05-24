using Animations.Parts;
using UnityEngine;

namespace Animations
{
    public class Tilt : NkmAnimation
    {
        private const float TimeToMove = 0.08f;
        
        public Tilt(Transform trans, string value, Color color)
        {
            AllowPlayingOtherAnimations = true;
            Vector3 tilt = new Vector3(0.5f, 0, 0.3f);
            AnimationParts.Enqueue(new MoveToPosition(trans, trans.position + tilt, TimeToMove));
            AnimationParts.Enqueue(new MoveToPosition(trans, trans.position, TimeToMove));
//            FloatingInfoStart f = new FloatingInfoStart(trans, value, color);
//            GameObject textObject = f.TextObject;
//            AnimationParts.Enqueue(f);
//            AnimationParts.Enqueue(new MoveToPosition(textObject.transform, textObject.transform.position + Vector3.forward * 15, 0.4f));
//            AnimationParts.Enqueue(new FloatingInfoFinish(textObject));
        }
        
    }
}