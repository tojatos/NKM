using System.Linq;
using Managers;
using NKMObjects.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterUI
{
	public class Face : SingletonMonoBehaviour<Face>
	{
		private Image _image;
		private static Game Game => GameStarter.Instance.Game;

		private void Awake() => _image = GetComponent<Image>();

		private void Update() //TODO: Remove Update
		{
			if (Game.Active.CharacterOnMap != null)
			{
				UpdateFace(Game.Active.CharacterOnMap);
			}
		}

		private void UpdateFace(Character characterOnMap)
		{
			Sprite characterSprite = Stuff.Sprites.CharacterHexagons.SingleOrDefault(s => s.name == characterOnMap.Name) ?? Stuff.Sprites.CharacterHexagons.Single(s => s.name == "Empty");
			_image.sprite = characterSprite;
			if(Game.HexMapDrawer.GetCharacterObject(characterOnMap)==null) return;
			Game.HexMapDrawer.GetCharacterObject(characterOnMap).transform.Find("Character Sprite").GetComponent<SpriteRenderer>().sprite = characterSprite;
		}
	}
}