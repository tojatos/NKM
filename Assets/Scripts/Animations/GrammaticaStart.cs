using System.Collections.Generic;
using System.Linq;
using Animations.Parts;
using Extensions;
using NKMObjects.Templates;
using UnityEngine;

namespace Animations
{
    public class GrammaticaStart : NkmAnimation
    {
        public GrammaticaStart(Transform ownerTransform, Character targetCharacter)
        {
            //TODO: Check somewhere in case of no neighbors
            Vector3 targetPosition = targetCharacter.ParentCell.GetNeighbors(1).GetRandom().transform.GetCharacterTransformPoint();
            AnimationParts.Enqueue(new TeleportToPosition(ownerTransform, targetPosition));  
            AnimationParts.Enqueue(new Wait(0.2f));  
        }
        
    }
}