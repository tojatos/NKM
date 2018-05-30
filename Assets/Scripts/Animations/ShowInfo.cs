using Animations.Parts;
using UnityEngine;

namespace Animations
{
    public class ShowInfo : NkmAnimation
    {
        
        public ShowInfo(Transform trans, string value, Color color)
        {
//            AllowPlayingOtherAnimations = true;
            var f = new FloatingInfoStart(trans, value, color);
            GameObject textObject = f.TextObject;
            AnimationParts.Enqueue(f);
            AnimationParts.Enqueue(new MoveToPosition(textObject.transform, textObject.transform.position + Vector3.forward * 15, 0.4f));
            AnimationParts.Enqueue(new FloatingInfoFinish(textObject));
        }
        
    }
}