using NKMCore.Templates;
using Unity.Animations.Parts;

namespace Unity.Animations
{
    public class Dim : NkmAnimation
    {
        public Dim(Character character)
        {
            var d = new DimCreate(character);
            AnimationParts.Enqueue(d);
            AnimationParts.Enqueue(new Show(d.HighlightObject));
        }

    }
}