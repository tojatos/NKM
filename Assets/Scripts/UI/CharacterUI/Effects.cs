using System.Collections.Generic;
using System.Linq;
using Extensions;
using Managers;
using NKMObjects.Templates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CharacterUI
{
	public class Effects : SingletonMonoBehaviour<Effects>
	{
		private static Game Game => GameStarter.Instance.Game;
		public GameObject EffectButtonPrefab;
		private List<GameObject> Buttons { get; } = new List<GameObject>();

		public void UpdateButtons()
		{
			if (Game.Active.CharacterOnMap == null) return;

			Character character = Game.Active.CharacterOnMap;
			RemoveButtons();
			character.Effects.ForEach(effect => CreateEffectButton(character, effect));
		}

		private static void SetButtonSprite(GameObject button, Effect effect)
		{
            Sprite effectSprite = Stuff.Sprites.Effects.SingleOrDefault(s => s.name == effect.ToString().Split('.').Last()) ?? Stuff.Sprites.Effects.Single(s => s.name == effect.GetEffectTypeName());
            button.GetComponent<Image>().sprite = effectSprite;
		}
		private void RemoveButtons()
		{
			if(Buttons == null) return;
			Buttons.ForEach(Destroy);
			Buttons.Clear();
		}
		private void CreateEffectButton(Character character, Effect effect)
		{
			GameObject button = Instantiate(EffectButtonPrefab, transform);

			button.name = character.Effects.IndexOf(effect).ToString();

			SetButtonSprite(button, effect);
			button.AddDefaultTooltip("<b>" + effect.Name + "</b>\n" + effect.GetDescription(), Tooltip.CharacterPosition);
			Buttons.Add(button);
		}
	}
}