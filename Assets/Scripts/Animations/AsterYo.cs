using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Animations
{
    public class AsterYo : NkmAnimation
    {
	    private const float ParticleSecondSize = 10f;
	    private readonly Transform _parentTransform;
	    private readonly List<Transform>  _targetTransforms;
	    
	    public AsterYo(Transform parentTransform, List<Transform> targetTransforms)
	    {
		    _parentTransform = parentTransform;
		    _targetTransforms = targetTransforms;
	    }
	    
        public override IEnumerator Play()
        {
            var particlesWithTargets = new Dictionary<GameObject, Transform>();
            foreach (var t in _targetTransforms)
            {
                var particle = Object.Instantiate(Stuff.Particles.Single(o => o.name == "Aster Yo"), _parentTransform.position, _parentTransform.rotation);

                particlesWithTargets.Add(particle, t);
//                PositionParticle(particle);
                particle.transform.localPosition += new Vector3(0,20,0);
            }

            yield return new WaitForSeconds(2f);

            foreach (var pair in particlesWithTargets)
            {
                var particle = pair.Key;
                var t = pair.Value;

                var main = particle.GetComponent<ParticleSystem>().main;
                main.startSize = new ParticleSystem.MinMaxCurve(ParticleSecondSize);
                AnimationPlayer.Add(new MoveToPosition(particle.transform, t.position, 0.1f));
            }

	        
			AnimationPlayer.CanPlayNext = true;
	        
            yield return new WaitForSeconds(1.5f);
            particlesWithTargets.ToList().ForEach(pair => Object.Destroy(pair.Key));
            
	        
        }
        
    }
}