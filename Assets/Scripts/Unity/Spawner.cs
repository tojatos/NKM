using System.Linq;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;
using Unity.Extensions;
using Unity.Hex;
using UnityEngine;

namespace Unity
{
	public class Spawner : SingletonMonoBehaviour<Spawner>
	{
		public GameObject CharacterPrefab;
		public GameObject HighlightPrefab;
		private void SpawnCharacterObject(DrawnHexCell parentCell, Character characterToSpawn)
		{
			Sprite characterSprite = Stuff.Sprites.CharacterHexagons.SingleOrDefault(s => s.name == characterToSpawn.Name) ?? Stuff.Sprites.CharacterHexagons.Single(s => s.name == "Empty");
			GameObject characterObject = Instantiate(CharacterPrefab, parentCell.transform);
			characterObject.name = characterToSpawn.Name;
			characterObject.transform.Find("Character Sprite").GetComponent<SpriteRenderer>().sprite = characterSprite;
			characterObject.transform.Find("Border").GetComponent<SpriteRenderer>().color = characterToSpawn.Owner.GetColor().ToUnityColor();
			characterObject.transform.localPosition = new Vector3(0, 10, 0);
			HexMapDrawer.Instance.SetCharacterObject(characterToSpawn, characterObject);
		}
		public void SpawnHighlightCellObject(DrawnHexCell parentCell, string colorName)
		{
			GameObject highlightObject = Instantiate(HighlightPrefab, parentCell.transform);
			highlightObject.transform.localPosition = new Vector3(0, 11, 0);
			highlightObject.GetComponent<SpriteRenderer>().sprite = Stuff.Sprites.HighlightHexagons.Single(s => s.name == colorName); 
			parentCell.Highlights.Add(highlightObject);
		}
		public void SpawnEffectHighlightCellObject(DrawnHexCell parentCell, string effectName)
		{
			GameObject highlightObject = Instantiate(HighlightPrefab, parentCell.transform);
			highlightObject.transform.localPosition = new Vector3(0, 1, 0);
			var sr = highlightObject.GetComponent<SpriteRenderer>();
			sr.sprite = Stuff.Sprites.HighlightHexagons.Single(s => s.name == effectName); 
			parentCell.EffectHighlights.Add(highlightObject);
		}
		public static bool CanSpawn(Character character, HexCell cell) => cell.IsFreeToStand && cell.IsSpawnFor(character.Owner);
		public void Spawn(DrawnHexCell cell, Character characterToSpawn)
		{
			SpawnCharacterObject(cell, characterToSpawn);
		}
	}
}