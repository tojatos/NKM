using System.Collections;
using UnityEngine;

namespace Animations.Parts
{
    public class Wait : NkmAnimationPart
    {
        private readonly float _timeToWait;
        
        public Wait(float timeToWait)
        {
            _timeToWait = timeToWait;
        }
        public override IEnumerator Play()
        {
            float t = 0f;
            while (t < 1)
            {
                t += Time.deltaTime / _timeToWait;
                yield return null;
            }

            IsFinished = true;
        }
    }
}
