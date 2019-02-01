using System.Collections;
using Unity.Extensions;
using UnityEngine;

namespace Unity.Animations.Parts
{
    public class Hide : NkmAnimationPart
    {
        private readonly GameObject _objectToHide;
        
        public Hide(GameObject objectToHide)
        {
            _objectToHide = objectToHide;
        }
        
        public override IEnumerator Play()
        {
            _objectToHide.Hide();

            IsFinished = true;
            yield break;
        }
    }
}
