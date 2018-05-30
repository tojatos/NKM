using System.Collections;
using UnityEngine;

namespace Animations.Parts
{
    public class Destroy : NkmAnimationPart
    {
	    private readonly GameObject _objectToDestroy;

        public Destroy(GameObject objectToDestroy)
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
