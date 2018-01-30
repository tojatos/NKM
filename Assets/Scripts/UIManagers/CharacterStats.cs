using Helpers;
using Managers;
using MyGameObjects.MyGameObject_templates;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIManagers
{
	public class CharacterStats : SingletonMonoBehaviour<CharacterStats>
	{
		private Game Game;


		//public Text CharacterName;
		public Text HealthPoints;
		public Text AttackPoints;
		public Text MagicalResistance;
		public Text PhysicalResistance;
		public Text Range;
		public Text Speed;
		public GameObject RangeObject;
		public GameObject SpeedObject;

		private void Awake()
		{
			Game = GameStarter.Instance.Game;

			EmptyTextes();
			SetAttackHelpTriggers();
			SetMoveHelpTriggers();
		}

		private void SetAttackHelpTriggers()
		{
			var trigger = RangeObject.GetComponent<EventTrigger>() ?? RangeObject.AddComponent<EventTrigger>();
			var entry = new EventTrigger.Entry {eventID = EventTriggerType.PointerEnter};
			entry.callback.AddListener((eventData) => Game.Active.HelpHexCells = Game.Active.CharacterOnMap.GetBasicAttackCells());
			trigger.triggers.Add(entry);

			var entry2 = new EventTrigger.Entry {eventID = EventTriggerType.PointerExit};
			entry2.callback.AddListener((eventData) => Game.Active.HelpHexCells = null);
			trigger.triggers.Add(entry2);
		}
		private void SetMoveHelpTriggers()
		{
			var trigger = SpeedObject.GetComponent<EventTrigger>() ?? SpeedObject.AddComponent<EventTrigger>();
			var entry = new EventTrigger.Entry {eventID = EventTriggerType.PointerEnter};
			entry.callback.AddListener((eventData) => Game.Active.HelpHexCells = Game.Active.CharacterOnMap.GetMoveCells());
			trigger.triggers.Add(entry);

			var entry2 = new EventTrigger.Entry {eventID = EventTriggerType.PointerExit};
			entry2.callback.AddListener((eventData) => Game.Active.HelpHexCells = null);
			trigger.triggers.Add(entry2);
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
			}
			else
			{
				EmptyTextes();
			}
		}


		private string GetStatText(Character character, StatType type)
		{
			var stat = character.GetStat(type);
			var bonus = stat.Value - stat.BaseValue;
			var bonusText = bonus > 0
				? "<color=green> + " + bonus + "</color>"
				: bonus < 0
					? "<color=red> - " + -bonus + "</color>"
					: "";
			return $"{stat.BaseValue}{bonusText}";
		}

	}
}