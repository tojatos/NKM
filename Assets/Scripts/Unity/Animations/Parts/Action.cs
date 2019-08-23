using System.Collections;
using Unity.Extensions;
using UnityEngine;

namespace Unity.Animations.Parts
{
    public class Action : NkmAnimationPart
    {
        private readonly System.Action _action;

        public Action(System.Action action)
        {
            _action = action;
        }

        public override IEnumerator Play()
        {
            _action?.Invoke();

            IsFinished = true;
            yield break;
        }
    }
}
