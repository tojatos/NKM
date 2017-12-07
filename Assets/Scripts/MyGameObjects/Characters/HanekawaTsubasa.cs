using System.Collections.Generic;
using MyGameObjects.Abilities.Hanekawa_Tsubasa;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Characters
{
	public class HanekawaTsubasa : Character
	{
		public HanekawaTsubasa()
		{
			Name = "Hanekawa Tsubasa";
			AttackPoints = new Stat(this, StatType.AttackPoints, 16);
			HealthPoints = new Stat(this, StatType.HealthPoints, 60);
			BasicAttackRange = new Stat(this, StatType.BasicAttackRange, 2);
			Speed = new Stat(this, StatType.Speed, 7);
			PhysicalDefense = new Stat(this, StatType.PhysicalDefense, 3);
			MagicalDefense = new Stat(this, StatType.MagicalDefense, 3);
			Type = FightType.Melee;
			InitiateAbilities(new List<Ability>
			{
				new NineLives(),
				new BloodKiss(),
				new CurseOfTheBlackCat()
			});

			Description = "";
			Quote = "";
			Author = "Jakub Mironowicz";
		}
	}
}
