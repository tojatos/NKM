using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Characters
{
	public class Crona : Character
	{
		public Crona()
		{
			Name = "Crona";
			AttackPoints = new Stat(this, StatType.AttackPoints, 14);
			HealthPoints = new Stat(this, StatType.HealthPoints, 79);
			BasicAttackRange = new Stat(this, StatType.BasicAttackRange, 4);
			Speed = new Stat(this, StatType.Speed, 4);
			PhysicalDefense = new Stat(this, StatType.PhysicalDefense, 2);
			MagicalDefense = new Stat(this, StatType.MagicalDefense, 4);
			Type = FightType.Melee;

			InitiateAbilities(null);

			Description = "";
			Quote = "Wiara w innych? Ufanie komuś, że cię nie skrzywdzi? Co za głupota...";
			Author = "Jakub Mironowicz";
		}

	}
}
