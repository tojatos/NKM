using System.Collections.Generic;
using System.Linq;
using Animations.Parts;
using UnityEngine;

namespace Animations
{
    public class GrammaticaFinish : NkmAnimation
    {
        public GrammaticaFinish(Transform ownerTransform, Transform targetTransform, Vector3 targetCellPosition)
        {
            AnimationParts.Enqueue(new Hide(ownerTransform.gameObject));  
            AnimationParts.Enqueue(new Hide(targetTransform.gameObject));  
            AnimationParts.Enqueue(new TeleportToPosition(ownerTransform, ownerTransform.parent.transform.TransformPoint(0,10,0)));
            AnimationParts.Enqueue(new TeleportToPosition(targetTransform, targetCellPosition));
            AnimationParts.Enqueue(new Wait(0.3f));  
            AnimationParts.Enqueue(new Show(ownerTransform.gameObject));  
            AnimationParts.Enqueue(new Show(targetTransform.gameObject));  
        }
    }
}