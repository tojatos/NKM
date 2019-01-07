﻿using Animations.Parts;
using Extensions;
using NKMObjects.Templates;
using UnityEngine;

namespace Animations
{
    public class GrammaticaStart : NkmAnimation
    {
        public GrammaticaStart(Transform ownerTransform, Character targetCharacter, GamePlayer friendlyPlayer)
        {
            //TODO: Check somewhere in case of no neighbors
            Vector3 targetPosition = Active.SelectDrawnCells(targetCharacter.ParentCell.GetNeighbors(friendlyPlayer, 1)).GetRandom()
                .transform.GetCharacterTransformPoint();
            AnimationParts.Enqueue(new TeleportToPosition(ownerTransform, targetPosition));  
            AnimationParts.Enqueue(new Wait(0.2f));  
        }
        
    }
}