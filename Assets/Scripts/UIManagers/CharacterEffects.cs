using System.Collections.Generic;
using MyGameObjects.MyGameObject_templates;
using UnityEngine;

namespace UIManagers
{
	public class CharacterEffects : SingletonMonoBehaviour<CharacterEffects>
	{
		public GameObject EffectButtonPrefab;
		public List<GameObject> Buttons { get; private set; }

		private void Awake()
		{
			Buttons = new List<GameObject>();
		}

		public void UpdateButtons()
		{
			if (Active.Instance.CharacterOnMap == null) return;

			var character = Active.Instance.CharacterOnMap;
			RemoveButtons();
			character.Effects.ForEach(effect => CreateEffectButton(character, effect));
		}

		private void RemoveButtons()
		{
			Buttons.ForEach(Destroy);
			Buttons.Clear();
		}
		private void CreateEffectButton(Character character, Effect effect)
		{
			var button = Instantiate(EffectButtonPrefab, transform);

			button.name = character.Effects.IndexOf(effect).ToString();

			button.AddSetTooltipEvent("<b>" + effect.Name + "</b>\n" + effect.GetDescription());
			button.AddRemoveTooltipEvent();
			//button.GetComponent<Button>().onClick.AddListener(effect.TryPrepare);
			Buttons.Add(button);
		}
	}
}