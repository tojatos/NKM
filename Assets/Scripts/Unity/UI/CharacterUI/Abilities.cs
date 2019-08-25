using System.Collections.Generic;
using System.Linq;
using NKMCore;
using NKMCore.Templates;
using Unity.Extensions;
using Unity.Hex;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unity.UI.CharacterUI
{
    /// <summary>
    /// Managing character ability buttons behaviour
    /// </summary>
    public class Abilities : SingletonMonoBehaviour<Abilities>
    {
        private static Game _game;

        public GameObject AbilityButtonPrefab;
        private List<GameObject> Buttons { get; set; }
        public void Init(Game game)
        {
            _game = game;
            Buttons = new List<GameObject>();
        }

        public void UpdateButtons()
        {
            if (_game?.Active.Character == null) return;

            Character character = _game.Active.Character;
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
                HexMapDrawer.ShowHelpHexCells(HexMapDrawer.Instance.SelectDrawnCells(ability.GetRangeCells()));
                ability.GetTargetsInRange().ForEach(c => HexMapDrawer.Instance.SelectDrawnCell(c).AddHighlight(Highlights.BlackTransparent));
            });
            button.AddTrigger(EventTriggerType.PointerExit, e =>
            {
                HexMapDrawer.Instance.HideHelpHexCells();
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
                        _game.Action.ClickAbility((IClickable) ability);
//                      Console.GameLog($"ABILITY CLICK: {ability.ID}");
                    }
                });

            }
            SetRangeHelpTriggers(button, ability);

            Buttons.Add(button);
        }
        public void UpdateButtonData()
        {
            if(_game?.Active.Character == null) return;

            Character character = _game.Active.Character;
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