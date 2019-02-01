using System.Collections.Generic;
using System.Linq;
using NKMCore;
using NKMCore.Hex;
using NKMCore.Templates;
using Unity.Extensions;
using Unity.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.UI.HexCellUI
{
	public class Effects : SingletonMonoBehaviour<Effects>
	{
		private static Game Game => GameStarter.Instance.Game;
		public GameObject HexEffectButtonPrefab;
		private List<GameObject> Buttons { get; set; }

		private void Awake() => Buttons = new List<GameObject>();

		private void Update() => UpdateButtons();

		public void UpdateButtons()
		{
			HexCell cell = Game.Active.SelectedCell;
			if (cell == null) return;
			RemoveButtons();
			cell.Effects.ForEach(effect => CreateEffectButton(cell, effect));
		}

		private static void SetButtonSprite(GameObject button, HexCellEffect effect)
		{
            Sprite effectSprite = Stuff.Sprites.Effects.SingleOrDefault(s => s.name == effect.ToString().Split('.').Last()) ?? Stuff.Sprites.Effects.Single(s => s.name == "Default Effect Sprite");
            button.GetComponent<Image>().sprite = effectSprite;
		}
		private void RemoveButtons()
		{
			if(Buttons == null) return;
			Buttons.ForEach(Destroy);
			Buttons.Clear();
		}
		private void CreateEffectButton(HexCell cell, HexCellEffect effect)
		{
			GameObject button = Instantiate(HexEffectButtonPrefab, transform);

			button.name = cell.Effects.IndexOf(effect).ToString();
			SetButtonSprite(button, effect);

			button.AddDefaultTooltip("<b>" + effect.Name + "</b>\n" + effect.GetDescription(), Tooltip.CharacterPosition);
			Buttons.Add(button);
		}
	}
}