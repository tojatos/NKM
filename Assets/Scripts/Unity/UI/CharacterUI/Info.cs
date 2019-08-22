using JetBrains.Annotations;
using Unity.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.UI.CharacterUI
{
	public class Info : SingletonMonoBehaviour<Info>
	{
//		private static Game Game => GameStarter.Instance.Game;
		public Text HealthPoints;
		public Text AttackPoints;
		public Text Range;
		public Text PhysicalResistance;
		public Text MagicalResistance;
		public Text Speed;
		public Text Description;
		public Text Quote;
		public Text Author;

		[UsedImplicitly]
		public void Open()
		{
			//TODO
//			gameObject.Show();
//			UpdateInfo(AllMyGameObjects.Characters.Single(c => c.Name == Game.Active.CharacterOnMap.Name));
		}

		private void Update()
		{
			if(Input.GetMouseButtonDown(1)) gameObject.Hide();
		}

//		private void UpdateInfo(Character character)
//		{
//			HealthPoints.text = character.HealthPoints.BaseValue.ToString();
//			AttackPoints.text = character.AttackPoints.ToString();
//			Range.text = character.BasicAttackRange.ToString();
//			PhysicalResistance.text = character.PhysicalDefense.ToString();
//			MagicalResistance.text = character.MagicalDefense.ToString();
//			Speed.text = character.Speed.ToString();
//			Description.text = character.Description;
//			Quote.text = character.Quote;
//			Author.text = character.Author;
//
//		}
	}
}