using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Extensions;
using UnityEngine;

namespace Unity.Animations.Parts
{
    public class AsterYoStart : NkmAnimationPart
    {
	    private const float ParticleSecondSize = 10f;
        public readonly Dictionary<GameObject, Transform> ParticlesWithTargets = new Dictionary<GameObject, Transform>();
        private readonly Transform _parentTransform;
//        private readonly List<Transform> _targetTransforms;

        public AsterYoStart(Transform parentTransform, List<Transform> targetTransforms)
        {
            _parentTransform = parentTransform;
//            _targetTransforms = targetTransforms;
            foreach (Transform targetTransform in targetTransforms)
            {
                GameObject particle = Object.Instantiate(Stuff.Particles.Single(o => o.name == "Aster Yo"), parentTransform.position, parentTransform.rotation);
                ParticlesWithTargets.Add(particle, targetTransform);
                particle.Hide();
            }
        }

        public override IEnumerator Play()
        {
            foreach (KeyValuePair<GameObject, Transform> particleWithTarget in ParticlesWithTargets)
            {
                GameObject particle = particleWithTarget.Key;
                particle.transform.position = _parentTransform.position;
                particle.transform.localPosition += new Vector3(0, 20, 0);
                particle.Show();
               
            }

            yield return new WaitForSeconds(2f);

            foreach (KeyValuePair<GameObject, Transform> particleWithTarget in ParticlesWithTargets)
            {
                ParticleSystem.MainModule main = particleWithTarget.Key.GetComponent<ParticleSystem>().main;
                main.startSize = new ParticleSystem.MinMaxCurve(ParticleSecondSize);
            }

            IsFinished = true;
        }
    }
}