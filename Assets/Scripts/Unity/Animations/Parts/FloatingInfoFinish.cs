using System.Collections;
using UnityEngine;

namespace Unity.Animations.Parts
{
    public class FloatingInfoFinish : NkmAnimationPart
    {
        private readonly GameObject _textObject;
        
        public FloatingInfoFinish(GameObject textObject)
        {
            _textObject = textObject;
        }

        public override IEnumerator Play()
        {
            yield return new WaitForSeconds(0.3f);
            Object.Destroy(_textObject);
            IsFinished = true;
        }
    }
}