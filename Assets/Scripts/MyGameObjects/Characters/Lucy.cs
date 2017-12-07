using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Characters
{
	public class Lucy : Character
	{
		public Lucy()
		{
			Name = "Lucy";
			AttackPoints = new Stat(this, StatType.AttackPoints, 10);
			HealthPoints = new Stat(this, StatType.HealthPoints, 82);
			BasicAttackRange = new Stat(this, StatType.BasicAttackRange, 2);
			Speed = new Stat(this, StatType.Speed, 4);
			PhysicalDefense = new Stat(this, StatType.PhysicalDefense, 5);
			MagicalDefense = new Stat(this, StatType.MagicalDefense, 3);
			Type = FightType.Melee;

			InitiateAbilities(null);

			Description = "";
			Quote = "Gdy jesteś beznadziejny, potrzebujesz kogoś jeszcze bardziej beznadziejnego, żeby poczuć się lepiej.";
			Author = "Jakub Mironowicz";
		}

	}
}
