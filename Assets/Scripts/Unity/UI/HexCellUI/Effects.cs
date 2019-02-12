using System.Collections.Generic;
using System.Linq;
using NKMCore.Hex;
using NKMCore.Templates;
using Unity.Extensions;
using Unity.Hex;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.UI.HexCellUI
{
	public class Effects : SingletonMonoBehaviour<Effects>
	{
		public GameObject HexEffectButtonPrefab;
		private List<GameObject> Buttons { get; set; }

		private void Awake()
		{
			Buttons = new List<GameObject>();
			HexMapDrawer.Instance.AfterCellSelect += UpdateButtons;
		}

		public void UpdateButtons(HexCell selectedCell)
		{
			if (selectedCell == null) return;
			RemoveButtons();
			selectedCell.Effects.ForEach(effect => CreateEffectButton(selectedCell, effect));
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