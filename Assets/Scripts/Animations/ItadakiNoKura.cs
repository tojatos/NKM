using Animations.Parts;
using UnityEngine;

namespace Animations
{
    public class ItadakiNoKura : NkmAnimation
    {
	    public ItadakiNoKura(Transform parentTransform, Transform targetTransform)
	    {
		    ItadakiNoKuraStart s = new ItadakiNoKuraStart(parentTransform, targetTransform);
		    AnimationParts.Enqueue(s);
		    GameObject particle = s._particle;
		    MoveToPosition m = new MoveToPosition(particle.transform, parentTransform, 2.5f);
		    m.IsFinished = true; //allow to play other animations
		    AnimationParts.Enqueue(m);
		    AnimationParts.Enqueue(new ItadakiNoKuraFinish(particle));
		    
	    }
        
    }
}