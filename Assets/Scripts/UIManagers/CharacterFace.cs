using System.Linq;
using MyGameObjects.MyGameObject_templates;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagers
{
	public class CharacterFace : SingletonMonoBehaviour<CharacterFace>
	{
		private Image Image;

		private void Awake()
		{
			Image = GetComponent<Image>();
		}

		private void Update() //TODO: Remove Update
		{
			if (Active.Instance.CharacterOnMap != null)
			{
				UpdateFace(Active.Instance.CharacterOnMap);
			}
		}

		private void UpdateFace(Character characterOnMap)
		{
			var characterSprite = Stuff.Sprites.CharacterHexagons.SingleOrDefault(s => s.name == characterOnMap.Name) ?? Stuff.Sprites.CharacterHexagons.Single(s => s.name == "Empty");
			Image.sprite = characterSprite;
			characterOnMap.CharacterObject.transform.Find("Character Sprite").GetComponent<SpriteRenderer>().sprite = characterSprite;
		}
	}
}