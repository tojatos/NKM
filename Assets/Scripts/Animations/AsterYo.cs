using System.Collections.Generic;
using Animations.Parts;
using UnityEngine;

namespace Animations
{
    public class AsterYo : NkmAnimation
    {
	    public AsterYo(Transform parentTransform, List<Transform> targetTransforms)
	    {
		    var asterYoParticlePart = new AsterYoStart(parentTransform, targetTransforms);
		    AnimationParts.Enqueue(asterYoParticlePart);
		    Dictionary<GameObject, Transform> particlesWithTargets = asterYoParticlePart.ParticlesWithTargets;
		    foreach (KeyValuePair<GameObject, Transform> pair in particlesWithTargets)
            {
                GameObject particle = pair.Key;
                Transform t = pair.Value;
	            AnimationParts.Enqueue(new MoveToPosition(particle.transform, t.position, 0.1f));
            }
		    AnimationParts.Enqueue(new AsterYoFinish(particlesWithTargets));
	    }
    }
}