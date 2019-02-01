using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.Animations.Parts
{
    public class AsterYoFinish : NkmAnimationPart
    {
        private readonly Dictionary<GameObject, Transform> _particlesWithTargets;

        public AsterYoFinish(Dictionary<GameObject, Transform> particlesWithTargets)
        {
            _particlesWithTargets = particlesWithTargets;
        }

        public override IEnumerator Play()
        {
            yield return new WaitForSeconds(1.5f);
            _particlesWithTargets.ToList().ForEach(pair => Object.Destroy(pair.Key));
            IsFinished = true;
        }
    }
}
