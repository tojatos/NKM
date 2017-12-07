using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Characters
{
	public class Shana : Character
	{
		public Shana()
		{
			Name = "Shana";
			AttackPoints = new Stat(this, StatType.AttackPoints, 0);
			HealthPoints = new Stat(this, StatType.HealthPoints, 73);
			BasicAttackRange = new Stat(this, StatType.BasicAttackRange, 4);
			Speed = new Stat(this, StatType.Speed, 5);
			PhysicalDefense = new Stat(this, StatType.PhysicalDefense, 4);
			MagicalDefense = new Stat(this, StatType.MagicalDefense, 5);
			Type = FightType.Melee;

			InitiateAbilities(null);

			Description = "";
			Quote = "Urusai, urusai, urusai!";
			Author = "Jakub Mironowicz";
		}

	}
}
