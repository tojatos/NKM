using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Helpers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Animations
{
    /// <summary>
    /// Create an animation from parts
    ///
    /// To create an animation, enqueue parts to AnimationParts queue.
    /// </summary>
    public abstract class NkmAnimation  
    {
        protected readonly Queue<NkmAnimationPart> AnimationParts = new Queue<NkmAnimationPart>();
        public bool AllowPlayingOtherAnimations { get; protected set; }
        
        /// <summary>
        /// Dequeues and plays every animation part from the queue, consecutively.
        /// </summary>
        public async Task Play()
        {
            while (AnimationParts.Count > 0)
            {
                NkmAnimationPart animationPart = AnimationParts.Dequeue();
                AnimationPlayer.Instance.StartCoroutine(animationPart.Play());
                Func<bool> isFinished = () => animationPart.IsFinished;
                await isFinished.WaitToBeTrue();
            }
        }

    }
}