using NKMCore.Templates;
using Unity.Animations.Parts;
using UnityEngine;

namespace Unity.Animations
{
    public class Check : NkmAnimation
    {
        public Check(Character character)
        {
            var d = new CheckCreate(character);
            GameObject king = d.KingObject;
            AnimationParts.Enqueue(d);
            AnimationParts.Enqueue(new Show(king));
            AnimationParts.Enqueue(new MoveToPosition(king.transform, king.transform.localPosition - new Vector3(0, 1, 0), 0.5f, false, true));
            AnimationParts.Enqueue(new Wait(1.3f));
            AnimationParts.Enqueue(new Hide(king));
        }

    }
}