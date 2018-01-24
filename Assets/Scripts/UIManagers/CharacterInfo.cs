using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Managers;
using MyGameObjects.MyGameObject_templates;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagers
{
	public class CharacterInfo : SingletonMonoBehaviour<CharacterInfo>
	{
		private Game Game;
		private Active Active;
		public Text HealthPoints;
		public Text AttackPoints;
		public Text Range;
		public Text PhysicalResistance;
		public Text MagicalResistance;
		public Text Speed;
		public Text Description;
		public Text Quote;
		public Text Author;

		void Awake()
		{
			Game = GameStarter.Instance.Game;
		}
		[UsedImplicitly]
		public void Open()
		{
			Game.UIManager.VisibleUI = new List<GameObject> { gameObject };
			UpdateInfo(AllMyGameObjects.Instance.Characters.Single(c => c.Name == Active.CharacterOnMap.Name));
		}

		private void UpdateInfo(Character character)
		{
			HealthPoints.text = character.HealthPoints.BaseValue.ToString();
			AttackPoints.text = character.AttackPoints.ToString();
			Range.text = character.BasicAttackRange.ToString();
			PhysicalResistance.text = character.PhysicalDefense.ToString();
			MagicalResistance.text = character.MagicalDefense.ToString();
			Speed.text = character.Speed.ToString();
			Description.text = character.Description;
			Quote.text = character.Quote;
			Author.text = character.Author;

		}
	}
}