using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Characters
{
	public class ItsukaKotori : Character
	{
		public ItsukaKotori()
		{
			Name = "Itsuka Kotori";
			AttackPoints = new Stat(this, StatType.AttackPoints, 14);
			HealthPoints = new Stat(this, StatType.HealthPoints, 55);
			BasicAttackRange = new Stat(this, StatType.BasicAttackRange, 5);
			Speed = new Stat(this, StatType.Speed, 5);
			PhysicalDefense = new Stat(this, StatType.PhysicalDefense, 3);
			MagicalDefense = new Stat(this, StatType.MagicalDefense, 5);
			Type = FightType.Melee;

			InitiateAbilities(null);

			Description = "";
			Quote = "Niech nasza randka się rozpocznie!";
			Author = "Jakub Mironowicz";
		}

	}
}
