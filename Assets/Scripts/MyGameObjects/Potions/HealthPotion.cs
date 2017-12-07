using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Potions
{
	public class HealthPotion : Potion
	{
		public HealthPotion()
		{
			Name = "Mikstura Życia";
			Description = "Wskrzesza wybraną postać z połową maksymalnego zdrowia (zaokrąglane w dół) na wybranym Bezpiecznym Punkcie będącym pod kontrolą gracza.";
		}
	}
}