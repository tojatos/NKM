using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Animations.Parts;
using UnityEngine;

namespace Animations
{
    public class AsterYo : NkmAnimation
    {
	    public AsterYo(Transform parentTransform, List<Transform> targetTransforms)
	    {
		    AsterYoStart asterYoParticlePart = new AsterYoStart(parentTransform, targetTransforms);
		    AnimationParts.Enqueue(asterYoParticlePart);
		    var particlesWithTargets = asterYoParticlePart.ParticlesWithTargets;
		    foreach (var pair in particlesWithTargets)
            {
                var particle = pair.Key;
                var t = pair.Value;
	            AnimationParts.Enqueue(new MoveToPosition(particle.transform, t.position, 0.1f));
            }
		    AnimationParts.Enqueue(new AsterYoFinish(particlesWithTargets));
	    }
	    
  
        
    }
}