using System.Collections.Generic;
using Extensions;
using Hex;
using Managers;
using NKMObjects.Templates;
using UnityEngine;

namespace UI.HexCellUI
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

			button.AddDefaultTooltip("<b>" + effect.Name + "</b>\n" + effect.GetDescription(), Tooltip.CharacterPosition);
			Buttons.Add(button);
		}
	}
}