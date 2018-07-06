using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using UnityEngine;

namespace Animations.Parts
{
    public class CamaelMegiddoStart : NkmAnimationPart
    {
        public readonly Dictionary<GameObject, Transform> LineParticlesWithTargets = new Dictionary<GameObject, Transform>();
        public readonly Dictionary<GameObject, Transform> ConflagrationParticlesWithTargets = new Dictionary<GameObject, Transform>();

        public CamaelMegiddoStart(IEnumerable<Transform> lineTransforms, IEnumerable<Transform> conflagrationTransforms)
        {
            foreach (Transform targetTransform in lineTransforms)
            {
                GameObject particle = Object.Instantiate(Stuff.Particles.Single(o => o.name == "Camael Megiddo Fire"), targetTransform);
                LineParticlesWithTargets.Add(particle, targetTransform);
                particle.Hide();
            }
            foreach (Transform targetTransform in conflagrationTransforms)
            {
                GameObject particle = Object.Instantiate(Stuff.Particles.Single(o => o.name == "Camael Megiddo Big Fire"), targetTransform);
                ConflagrationParticlesWithTargets.Add(particle, targetTransform);
                particle.Hide();
            }
        }

        public override IEnumerator Play()
        {
            foreach (KeyValuePair<GameObject, Transform> particleWithTarget in LineParticlesWithTargets)
            {
                GameObject particle = particleWithTarget.Key;
                particle.transform.position = particleWithTarget.Value.position;
                particle.transform.localPosition += new Vector3(0, 20, 0);
                particle.Show();
                yield return new WaitForSeconds(0.1f);
            }
            
            foreach (KeyValuePair<GameObject, Transform> particleWithTarget in ConflagrationParticlesWithTargets)
            {
                GameObject particle = particleWithTarget.Key;
                particle.transform.position = particleWithTarget.Value.position;
                particle.transform.localPosition += new Vector3(0, 20, 0);
                particle.Show();
            }

            IsFinished = true;
        }
    }
}