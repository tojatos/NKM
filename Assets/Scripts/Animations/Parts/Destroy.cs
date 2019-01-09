using System.Collections;
using UnityEngine;

namespace Animations.Parts
{
    public class Destroy : NkmAnimationPart
    {
	    private readonly Object _objectToDestroy;

        public Destroy(Object objectToDestroy)
        {
            _objectToDestroy = objectToDestroy;
        }

        public override IEnumerator Play()
        {
            Object.Destroy(_objectToDestroy);
            IsFinished = true;
            yield break;
        }
    }
}
