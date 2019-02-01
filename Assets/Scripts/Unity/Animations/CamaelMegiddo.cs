using System.Collections.Generic;
using System.Linq;
using Unity.Animations.Parts;
using UnityEngine;

namespace Unity.Animations
{
    public class CamaelMegiddo : NkmAnimation
    {
        public CamaelMegiddo(IEnumerable<Transform> lineTransforms, IEnumerable<Transform> conflagrationTransforms)
        {
            var cms = new CamaelMegiddoStart(lineTransforms, conflagrationTransforms);
            AnimationParts.Enqueue(cms);  
            AnimationParts.Enqueue(new DestroyParticles(cms.LineParticlesWithTargets.Select(t => t.Key).ToList(), 0.1f, 1.5f, false));
            AnimationParts.Enqueue(new DestroyParticles(cms.ConflagrationParticlesWithTargets.Select(t => t.Key).ToList(), 0.1f, cms.LineParticlesWithTargets.Count * 0.1f + 1.5f));
        }
        
    }
}