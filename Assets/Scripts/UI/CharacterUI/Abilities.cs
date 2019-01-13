using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using Managers;
using NKMObjects.Templates;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.CharacterUI
{
	/// <summary>
	/// Managing character ability buttons behaviour
	/// </summary>
	public class Abilities : SingletonMonoBehaviour<Abilities>
	{
		private static Game Game => GameStarter.Instance.Game;
		private Console Console => Game.Console;

		public GameObject AbilityButtonPrefab;
		private List<GameObject> Buttons { get; set; }
		public void Init() => Buttons = new List<GameObject>();

		public void UpdateButtons()
		{
			if (Game.Active.Character == null) return;

			Character character = Game.Active.Character;
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
		private static void SetRangeHelpTriggers(GameObject button, Ability ability)
		{
			button.AddTrigger(EventTriggerType.PointerEnter, e =>
			{
				Game.Active.HelpHexCells = ability.GetRangeCells();
				ability.GetTargetsInRange().ForEach(c => Active.SelectDrawnCell(c).AddHighlight(Highlights.BlackTransparent));
			});
			button.AddTrigger(EventTriggerType.PointerExit, e =>
			{
				Game.Active.HelpHexCells = null;
				HexMapDrawer.Instance.RemoveHighlightsOfColor(Highlights.BlackTransparent);
			});
		}
		private static void SetButtonSprite(GameObject button, Ability ability)
		{
				Sprite abilitySprite = Stuff.Sprites.Abilities.SingleOrDefault(s => s.name == ability.Name) ?? Stuff.Sprites.Abilities.Single(s => s.name == "Default Ability Sprite");
				button.GetComponent<Image>().sprite = abilitySprite;
		}
		private void CreateAbilityButton(Character character, Ability ability)
		{
			GameObject button = Instantiate(AbilityButtonPrefab, transform);
			button.name = character.Abilities.IndexOf(ability).ToString(); //we do that to be able to UpdateButtonData

			button.AddSetTooltipEvent("<b>" + ability.Name + "</b>\n" + ability.GetDescription(), Tooltip.CharacterPosition);
			button.AddRemoveTooltipEvent();

			SetButtonSprite(button, ability);
			if (ability is IClickable)
			{
				button.AddTrigger(EventTriggerType.PointerClick, e =>
				{
					if (ability.CanBeUsed)
					{
						((IClickable) ability).Click();
						Console.GameLog($"ABILITY CLICK: {ability.ID}");
					}	
				});
//                button.GetComponent<Button>().onClick.AddListener(()=>
//                {
//                    if(ability.CanBeUsed) ((IClickable) ability).ImageClick();
//    				ability.TryPrepare();
//                }); //try to prepare an ability on click
                    
			}
			SetRangeHelpTriggers(button, ability);

			Buttons.Add(button);
		}
		public void UpdateButtonData()
		{
			if(Game?.Active.Character == null) return;

			Character character = Game.Active.Character;
			Buttons.ForEach(button =>
			{
				Ability ability = character.Abilities[int.Parse(button.name)];
				button.ChangeImageColor(!ability.CanBeUsed ? Color.grey : Color.white);
				button.GetComponentInChildren<Text>().text = ability.CurrentCooldown > 0 ? ability.CurrentCooldown.ToString() : "";

				CreateEnableSpriteIfEnableable(button, ability);
			});
		}
		private static void CreateEnableSpriteIfEnableable(GameObject button, Ability ability)
		{
			if (!(ability is IEnableable)) return;

			var enableableAbility = (IEnableable) ability;
			var enableGameObject = new GameObject();
			enableGameObject.transform.parent = button.transform;
			enableGameObject.AddComponent<Image>().sprite = Stuff.Sprites.Icons.Find(s => s.name == (enableableAbility.IsEnabled ? "Ability Active" : "Ability Inactive"));

			var rect = enableGameObject.GetComponent<RectTransform>();
			rect.anchorMin = new Vector2(1, 1);
			rect.anchorMax = new Vector2(1, 1);
			rect.anchoredPosition = new Vector3(-7.5f, -7.5f, 0);
			rect.sizeDelta = new Vector2(15, 15);
		}

	}
}