using NKMCore;
using NKMCore.Extensions;
using NKMCore.Templates;
using Unity.Extensions;
using Unity.Hex;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Unity.UI.CharacterUI
{
    public class Stats : SingletonMonoBehaviour<Stats>
    {
        private Game _game;

        public Text HealthPoints;
        public Text AttackPoints;
        public Text MagicalResistance;
        public Text PhysicalResistance;
        public Text Range;
        public Text Speed;
        public GameObject RangeObject;
        public GameObject SpeedObject;

        public void Init(Game game)
        {
            _game = game;
            EmptyTextes();
            SetTooltipTriggers();
            SetAttackHelpTriggers();
            SetMoveHelpTriggers();
        }

        private void SetTooltipTriggers()
        {
            HealthPoints.transform.parent.gameObject.AddDefaultTooltip("Punkty życia");
            AttackPoints.transform.parent.gameObject.AddDefaultTooltip("Punkty ataku");
            MagicalResistance.transform.parent.gameObject.AddDefaultTooltip("Obrona magiczna");
            PhysicalResistance.transform.parent.gameObject.AddDefaultTooltip("Obrona fizyczna");
            Range.transform.parent.gameObject.AddDefaultTooltip("Zasięg podstawowego ataku");
            Speed.transform.parent.gameObject.AddDefaultTooltip("Szybkość");
        }

        private void SetAttackHelpTriggers()
        {
            RangeObject.AddTrigger(EventTriggerType.PointerEnter, e => HexMapDrawer.ShowHelpHexCells(HexMapDrawer.Instance.SelectDrawnCells(_game.Active.Character.GetBasicAttackCells())));
            RangeObject.AddTrigger(EventTriggerType.PointerExit, e => HexMapDrawer.Instance.HideHelpHexCells());
        }
        private void SetMoveHelpTriggers()
        {
            SpeedObject.AddTrigger(EventTriggerType.PointerEnter, e => HexMapDrawer.ShowHelpHexCells(HexMapDrawer.Instance.SelectDrawnCells(_game.Active.Character.GetBasicMoveCells())));
            SpeedObject.AddTrigger(EventTriggerType.PointerExit, e => HexMapDrawer.Instance.HideHelpHexCells());
        }
        private void EmptyTextes()
        {
            HealthPoints.text = "";
            AttackPoints.text = "";
            MagicalResistance.text = "";
            PhysicalResistance.text = "";
            Range.text = "";
            Speed.text = "";
        }
        public void UpdateCharacterStats(Character character)
        {
            if (character != null)
            {
                AttackPoints.text = GetStatText(character, StatType.AttackPoints);
                HealthPoints.text = character.HealthPoints + "/" + character.HealthPoints.BaseValue;
                MagicalResistance.text = GetStatText(character, StatType.MagicalDefense);
                PhysicalResistance.text = GetStatText(character, StatType.PhysicalDefense);
                Range.text = GetStatText(character, StatType.BasicAttackRange);
                Speed.text = GetStatText(character, StatType.Speed);
                if (character.Shield.Value > 0) HealthPoints.text += "<color=green> + " + character.Shield.Value + "</color>";
            }
            else
                EmptyTextes();
        }


        private static string GetStatText(Character character, StatType type)
        {
            Stat stat = character.GetStat(type);
            int bonus = stat.Value - stat.BaseValue;
            string bonusText = bonus > 0
                ? "<color=green> + " + bonus + "</color>"
                : bonus < 0
                    ? "<color=red> - " + -bonus + "</color>"
                    : "";
            return $"{stat.BaseValue}{bonusText}";
        }

    }
}