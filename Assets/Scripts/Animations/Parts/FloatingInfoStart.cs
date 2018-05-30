using System.Collections;
using System.Linq;
using Helpers;
using UnityEngine;

namespace Animations.Parts
{
    public class FloatingInfoStart : NkmAnimationPart
    {
        private readonly string _textToShow;
        private readonly Color _textColor;
        public readonly GameObject TextObject;
        
        public FloatingInfoStart(Transform trans, string value, Color color)
        {
            _textToShow = value;
            _textColor = color;
            TextObject = Object.Instantiate(Stuff.Prefabs.Single(p => p.name == "Floating Info"), trans.position, trans.rotation);
            TextObject.Hide();
        }

        public override IEnumerator Play()
        {
            var mesh = TextObject.GetComponent<TextMesh>();
            mesh.color = _textColor;
            mesh.text = _textToShow;
            mesh.fontSize = 120;
            TextObject.Show();
            
            IsFinished = true;
            yield break;
        }
    }
}