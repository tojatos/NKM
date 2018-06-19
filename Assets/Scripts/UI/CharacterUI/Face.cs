﻿using System.Linq;
using Managers;
using NKMObjects.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterUI
{
	public class Face : SingletonMonoBehaviour<Face>
	{
		private Image Image;
		private Game Game;

		private void Awake()
		{
			Game = GameStarter.Instance.Game;
			Image = GetComponent<Image>();
		}

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
			Image.sprite = characterSprite;
			characterOnMap.CharacterObject.transform.Find("Character Sprite").GetComponent<SpriteRenderer>().sprite = characterSprite;
		}
	}
}