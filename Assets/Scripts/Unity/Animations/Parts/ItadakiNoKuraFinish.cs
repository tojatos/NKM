using System.Collections;
using UnityEngine;

namespace Unity.Animations.Parts
{
    public class ItadakiNoKuraFinish : NkmAnimationPart
    {
        private readonly GameObject _particle;

        public ItadakiNoKuraFinish(GameObject particle)
        {
            _particle = particle;
        }

        public override IEnumerator Play()
        {
            IsFinished = true; // On beginning to allow playing other animations
            yield return new WaitForSeconds(3.5f);
            Object.Destroy(_particle);
        }
    }
}
