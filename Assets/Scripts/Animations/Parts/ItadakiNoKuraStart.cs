using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using UnityEngine;

namespace Animations.Parts
{
    public class ItadakiNoKuraStart : NkmAnimationPart
    {
        private const float ParticleStartSize = 20f;
	    private readonly Transform _parentTransform;
	    private readonly Transform _targetTransform;
	    public readonly GameObject _particle;
	    
	    public ItadakiNoKuraStart(Transform parentTransform, Transform targetTransform)
	    {
		    _parentTransform = parentTransform;
		    _targetTransform = targetTransform;
            _particle = Object.Instantiate(Stuff.Particles.Single(o => o.name == "Itadaki No Kura"), _targetTransform.position, _targetTransform.rotation);
		    _particle.Hide();
	    }
	    
        public override IEnumerator Play()
        {
            _particle.transform.localPosition += new Vector3(0,20,0);
            var main = _particle.GetComponent<ParticleSystem>().main;
            main.startSize = new ParticleSystem.MinMaxCurve(ParticleStartSize);
	        _particle.Show();
	        
	        IsFinished = true;
	        yield break; 
        }
    }
}
