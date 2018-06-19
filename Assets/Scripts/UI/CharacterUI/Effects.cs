using System.Collections.Generic;
using Extensions;
using Managers;
using NKMObjects.Templates;
using UnityEngine;

namespace UI.CharacterUI
{
	public class Effects : SingletonMonoBehaviour<Effects>
	{
		private Game Game => GameStarter.Instance.Game;
		public GameObject EffectButtonPrefab;
		public List<GameObject> Buttons { get; private set; }

		private void Awake()
		{
//			Game = GameStarter.Instance.Game;
			Buttons = new List<GameObject>();
		}

		public void UpdateButtons()
		{
			if (Game.Active.CharacterOnMap == null) return;

			Character character = Game.Active.CharacterOnMap;
			RemoveButtons();
			character.Effects.ForEach(effect => CreateEffectButton(character, effect));
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

			button.AddSetTooltipEvent("<b>" + effect.Name + "</b>\n" + effect.GetDescription());
			button.AddRemoveTooltipEvent();
			//button.GetComponent<Button>().onClick.AddListener(effect.TryPrepare);
			Buttons.Add(button);
		}
	}
}