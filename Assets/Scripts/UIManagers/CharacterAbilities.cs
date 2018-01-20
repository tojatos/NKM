using System.Collections.Generic;
using System.Linq;
using MyGameObjects.MyGameObject_templates;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIManagers
{
	/// <summary>
	/// Managing character ability buttons behaviour
	/// </summary>
	public class CharacterAbilities : SingletonMonoBehaviour<CharacterAbilities>
	{
		public GameObject AbilityButtonPrefab;
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

			//Create an ability button for each character ability
			character.Abilities.ForEach(ability => CreateAbilityButton(character, ability));
			UpdateButtonData();
		}
		/// <summary>
		/// Removes buttons from the list and scene.
		/// </summary>
		private void RemoveButtons()
		{
			Buttons.ForEach(Destroy);
			Buttons.Clear();
		}
		/// <summary>
		/// Show range using Help Hex Cells on the board, while hovering over button.
		/// </summary>
		/// <param name="button">Show range after hovering over this button</param>
		/// <param name="ability">Get range from this ability</param>
		private void SetRangeHelpTriggers(GameObject button, Ability ability)
		{
			var trigger = button.GetComponent<EventTrigger>() ?? button.AddComponent<EventTrigger>();
			var entry = new EventTrigger.Entry {eventID = EventTriggerType.PointerEnter};
			entry.callback.AddListener(eventData => Active.Instance.HelpHexCells = ability.GetRangeCells());
			trigger.triggers.Add(entry);
			var entry2 = new EventTrigger.Entry {eventID = EventTriggerType.PointerExit};
			entry2.callback.AddListener(eventData => Active.Instance.HelpHexCells = null);
			trigger.triggers.Add(entry2);
		}
		private void SetButtonSprite(GameObject button, Ability ability)
		{
				var abilitySprite = Stuff.Sprites.Abilities.SingleOrDefault(s => s.name == ability.Name) ?? Stuff.Sprites.Abilities.Single(s => s.name == "Default Ability Sprite");
				button.GetComponent<Image>().sprite = abilitySprite;
		}
		private void CreateAbilityButton(Character character, Ability ability)
		{
			var button = Instantiate(AbilityButtonPrefab, transform);
			button.name = character.Abilities.IndexOf(ability).ToString(); //we do that to be able to UpdateButtonData

			button.AddSetTooltipEvent("<b>" + ability.Name + "</b>\n" + ability.GetDescription());
			button.AddRemoveTooltipEvent();

			SetButtonSprite(button, ability);
			button.GetComponent<Button>().onClick.AddListener(ability.TryPrepare); //try to prepare an ability on click
			SetRangeHelpTriggers(button, ability);

			Buttons.Add(button);
		}
		public void UpdateButtonData()
		{
			if(Active.Instance.CharacterOnMap==null) return;

			var character = Active.Instance.CharacterOnMap;
			Buttons.ForEach(button =>
			{
				var ability = character.Abilities[int.Parse(button.name)];
				button.ChangeImageColor(!ability.CanUse ? Color.grey : Color.white);
				button.GetComponentInChildren<Text>().text = ability.CurrentCooldown > 0 ? ability.CurrentCooldown.ToString() : "";

				CreateEnableSpriteIfEnableable(button, ability);
			});
		}
		private void CreateEnableSpriteIfEnableable(GameObject button, Ability ability)
		{
			if (!(ability is EnableableAbility)) return;

			var enableableAbility = (EnableableAbility) ability;
			var enableGameObject = new GameObject();
			enableGameObject.transform.parent = button.transform;
			enableGameObject.AddComponent<Image>().sprite = Stuff.Sprites.Icons.Find(s => s.name == (enableableAbility.IsEnabled ? "Ability Active" : "Ability Inactive"));

			var rect = enableGameObject.GetComponent<RectTransform>();
			rect.anchorMin = new Vector2(1, 1);
			rect.anchorMax = new Vector2(1, 1);
			rect.anchoredPosition = new Vector3(-15, -15, 0);
			rect.sizeDelta = new Vector2(30, 30);
		}

	}
}