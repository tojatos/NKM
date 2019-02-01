using UnityEngine;

namespace Unity.Animations
{
    public class Destroy : NkmAnimation
    {
        public Destroy(Object objectToDestroy)
        {
            AnimationParts.Enqueue(new Parts.Destroy(objectToDestroy));
        }
        
        public Destroy(Object objectToDestroy, float delay)
        {
            AnimationParts.Enqueue(new Parts.Wait(delay));
            AnimationParts.Enqueue(new Parts.Destroy(objectToDestroy));
        }
    }
}