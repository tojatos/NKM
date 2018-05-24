using System.Collections;

namespace Animations
{
    public abstract class NkmAnimationPart
    {
        /// <summary>
        /// Plays an animation part.
        ///
        /// It should contain`IsFinished = true`.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator Play();
        public bool IsFinished;
    }
}