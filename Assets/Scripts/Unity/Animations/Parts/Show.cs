using System.Collections;
using Unity.Extensions;
using UnityEngine;

namespace Unity.Animations.Parts
{
    public class Show : NkmAnimationPart
    {
        private readonly GameObject _objectToShow;
        
        public Show(GameObject objectToShow)
        {
            _objectToShow = objectToShow;
        }
        
        public override IEnumerator Play()
        {
            _objectToShow.Show();

            IsFinished = true;
            yield break;
        }
    }
}
