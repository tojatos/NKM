using System.Collections;
using System.Linq;
using NKMCore.Templates;
using Unity.Extensions;
using Unity.Hex;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unity.Animations.Parts
{
    public class DimCreate : NkmAnimationPart
    {
        public readonly GameObject HighlightObject;

        public DimCreate(Character character)
        {
            GameObject characterObject = HexMapDrawer.Instance.GetCharacterObject(character);
            if(characterObject == null) return;
			HighlightObject = Object.Instantiate(Stuff.Prefabs.Find(s => s.name == "Highlight"), characterObject.transform);
			HighlightObject.transform.localPosition = new Vector3(0, 0, -5);
            HighlightObject.transform.rotation = new Quaternion(0, 0, 0, 0);
            HighlightObject.transform.localScale = new Vector3(1,1, 1);
            HighlightObject.GetComponent<SpriteRenderer>().sprite = Stuff.Sprites.HighlightHexagons.Single(s => s.name == "Dim");

            HexMapDrawer.Dims[character] = HighlightObject;

            HighlightObject.Hide();
        }

        public override IEnumerator Play()
        {
            IsFinished = true;
            yield break;
        }
    }
}