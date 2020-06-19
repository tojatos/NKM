using System.Collections;
using System.Linq;
using Unity.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unity.Animations.Parts
{
    public class VanishablePopupCreate : NkmAnimationPart
    {
        public readonly GameObject PopupObject;

        public VanishablePopupCreate(string message)
        {
            GameObject px = Stuff.Prefabs.Single(p => p.name == "Vanishable Popup");
            Transform handle = GameObject.Find("UIManager").transform;
            PopupObject = Object.Instantiate(px, handle);
            var popup = PopupObject.GetComponent<VanishablePopup>();
            popup.Message.text = message;
            PopupObject.Hide();
        }

        public override IEnumerator Play()
        {

            IsFinished = true;
            yield break;
        }
    }
}