using System.Linq;
using NKMCore;
using NKMCore.Templates;
using Unity.Hex;
using Unity.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.UI.CharacterUI
{
	public class Face : SingletonMonoBehaviour<Face>
	{
		private Image _image;
		private static Game Game => GameStarter.Instance.Game;

		private void Awake() => _image = GetComponent<Image>();

		private void Update() //TODO: Remove Update
		{
			if (Game.Active.Character != null)
			{
				UpdateFace(Game.Active.Character);
			}
		}

		private void UpdateFace(Character characterOnMap)
		{
			Sprite characterSprite = Stuff.Sprites.CharacterHexagons.SingleOrDefault(s => s.name == characterOnMap.Name) ?? Stuff.Sprites.CharacterHexagons.Single(s => s.name == "Empty");
			_image.sprite = characterSprite;
			if(HexMapDrawer.Instance.GetCharacterObject(characterOnMap)==null) return;
			HexMapDrawer.Instance.GetCharacterObject(characterOnMap).transform.Find("Character Sprite").GetComponent<SpriteRenderer>().sprite = characterSprite;
		}
	}
}