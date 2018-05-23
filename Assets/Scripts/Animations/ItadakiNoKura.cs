using System.Collections;
using System.Linq;
using UnityEngine;

namespace Animations
{
    public class ItadakiNoKura : NkmAnimation
    {
	    private const float ParticleStartSize = 20f;
	    private const float AnimationDuration = 3.5f;
	    private readonly Transform _parentTransform;
	    private readonly Transform _targetTransform;
	    private readonly bool _allowPlayingOtherAnimations;
	    
	    public ItadakiNoKura(Transform parentTransform, Transform targetTransform, bool allowPlayingOtherAnimaitions = true)
	    {
		    _parentTransform = parentTransform;
		    _targetTransform = targetTransform;
		    _allowPlayingOtherAnimations = allowPlayingOtherAnimaitions;
	    }
	    
        public override IEnumerator Play()
        {
	        if (_allowPlayingOtherAnimations) AnimationPlayer.CanPlayNext = true;
            var particle = Object.Instantiate(Stuff.Particles.Single(o => o.name == "Itadaki No Kura"), _targetTransform.position, _targetTransform.rotation);

            particle.transform.localPosition += new Vector3(0,20,0);
            var main = particle.GetComponent<ParticleSystem>().main;
            main.startSize = new ParticleSystem.MinMaxCurve(ParticleStartSize);
	        
	        //Start animation of moving the particle
	        AnimationPlayer.Add(new MoveToPosition(particle.transform, _parentTransform.position, AnimationDuration-1f, false));
	        AnimationPlayer.CanPlayNext = true;
	        
	        //Destroy the particle after the animation finishes
            yield return new WaitForSeconds(AnimationDuration);
            Object.Destroy(particle);
        }
        
    }
}