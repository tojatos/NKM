using Unity.Animations.Parts;
using UnityEngine;

namespace Unity.Animations
{
    public class ItadakiNoKura : NkmAnimation
    {
	    public ItadakiNoKura(Transform parentTransform, Transform targetTransform)
	    {
		    var s = new ItadakiNoKuraStart(targetTransform);
		    AnimationParts.Enqueue(s);
		    GameObject particle = s.Particle;
		    var m = new MoveToPosition(particle.transform, parentTransform, 2.5f) {IsFinished = true};
		    //allow to play other animations
		    AnimationParts.Enqueue(m);
		    AnimationParts.Enqueue(new ItadakiNoKuraFinish(particle));
		    
	    }
        
    }
}