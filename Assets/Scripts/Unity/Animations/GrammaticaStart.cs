using NKMCore.Extensions;
using NKMCore.Templates;
using Unity.Animations.Parts;
using Unity.Extensions;
using Unity.Hex;
using UnityEngine;

namespace Unity.Animations
{
    public class GrammaticaStart : NkmAnimation
    {
        public GrammaticaStart(Character parentCharacter, Character targetCharacter)
        {
            //TODO: Check somewhere in case of no neighbors
            Transform ownerTransform = HexMapDrawer.Instance.GetCharacterObject(parentCharacter).transform;
            Vector3 targetPosition = HexMapDrawer.Instance.SelectDrawnCells(targetCharacter.ParentCell.GetNeighbors(parentCharacter.Owner, 1)).GetRandom()
                .transform.GetCharacterTransformPoint();
            AnimationParts.Enqueue(new TeleportToPosition(ownerTransform, targetPosition));  
            AnimationParts.Enqueue(new Wait(0.2f));  
        }
        
    }
}