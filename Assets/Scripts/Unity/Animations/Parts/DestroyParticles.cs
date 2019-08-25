using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Animations.Parts
{
    public class DestroyParticles : NkmAnimationPart
    {
        private readonly List<GameObject> _particles;
        private readonly float _animationTime;
        private readonly float _delay;
        private readonly bool _simultanously;

        public DestroyParticles(List<GameObject> particles, float animationTime, float delay = 0, bool simultanously = true)
        {
            _particles = particles;
            _animationTime = animationTime;
            _delay = delay;
            _simultanously = simultanously;
        }

        public override IEnumerator Play()
        {
            if (_simultanously)
            {
                yield return new WaitForSeconds(_delay);
                yield return new WaitForSeconds(_animationTime);
                _particles.ForEach(Object.Destroy);
            }
            else
            {
                yield return new WaitForSeconds(_delay);
                foreach (GameObject p in _particles)
                {
                    yield return new WaitForSeconds(_animationTime);
                    Object.Destroy(p);
                }
            }
            IsFinished = true;
        }
    }
}
