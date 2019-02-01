using Unity.Animations.Parts;
using UnityEngine;

namespace Unity.Animations
{
    public class ShowInfo : NkmAnimation
    {
        
        public ShowInfo(Transform targetTransform, string value, Color color)
        {
//            AllowPlayingOtherAnimations = true;
            var f = new FloatingInfoStart(targetTransform, value, color);
            GameObject textObject = f.TextObject;
            AnimationParts.Enqueue(f);
            AnimationParts.Enqueue(new TeleportToPosition(textObject.transform, targetTransform)); //update position if targetTransform has moved
            AnimationParts.Enqueue(new TeleportToPosition(textObject.transform, textObject.transform.position + Vector3.up)); //show info above the object
            AnimationParts.Enqueue(new MoveToPosition(textObject.transform, Vector3.forward * 15, 0.4f, true));
            AnimationParts.Enqueue(new FloatingInfoFinish(textObject));
        }
        
    }
}