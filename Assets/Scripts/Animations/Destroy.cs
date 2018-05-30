using UnityEngine;

namespace Animations
{
    public class Destroy : NkmAnimation
    {
        public Destroy(GameObject objectToDestroy)
        {
            AnimationParts.Enqueue(new Parts.Destroy(objectToDestroy));
        }
        
    }
}