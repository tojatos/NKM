using System.Collections;
using System.Linq;
using NKMCore.Templates;
using Unity.Extensions;
using Unity.Hex;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unity.Animations.Parts
{
    public class CheckCreate : NkmAnimationPart
    {
        public readonly GameObject KingObject;

        public CheckCreate(Character character)
        {
            GameObject characterObject = HexMapDrawer.Instance.GetCharacterObject(character);
            if(characterObject == null) return;
            KingObject = Object.Instantiate(new GameObject(), characterObject.transform);
            KingObject.AddComponent<SpriteRenderer>();
            KingObject.GetComponent<SpriteRenderer>().sprite = Stuff.Sprites.Icons.Find(s => s.name == "ChessKing");
            KingObject.transform.localPosition = new Vector3(0, 3.5f, -7);
            KingObject.transform.rotation = new Quaternion(0, 0, 0, 0);
            KingObject.transform.localScale = new Vector3(0.3f ,0.3f, 0.3f);

            KingObject.Hide();
        }

        public override IEnumerator Play()
        {
            IsFinished = true;
            yield break;
        }
    }
}