using System.Linq;
using Unity.Animations.Parts;
using UnityEngine;

namespace Unity.Animations
{
    public class ShowVanishablePopup : NkmAnimation
    {
        public ShowVanishablePopup(string message, int time)
        {

            var f = new VanishablePopupCreate(message);
            GameObject popupObject = f.PopupObject;
            AnimationParts.Enqueue(f);
            AnimationParts.Enqueue(new Show(popupObject));
            AnimationParts.Enqueue(new Wait(time));
            AnimationParts.Enqueue(new Hide(popupObject));
        }

    }
}