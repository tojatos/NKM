using NKMCore.Templates;
using Unity.Animations.Parts;
using Unity.Hex;
using UnityEngine;

namespace Unity.Animations
{
    public class Undim : NkmAnimation
    {
        public Undim(Character character)
        {
            if(!HexMapDrawer.Dims.ContainsKey(character)) return;
            GameObject d = HexMapDrawer.Dims[character];
            HexMapDrawer.Dims.Remove(character);
            AnimationParts.Enqueue(new Hide(d));
        }

    }
}