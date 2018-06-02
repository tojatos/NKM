using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Effects
{
	public class GurenNoSouyoku : Effect
	{
		public GurenNoSouyoku(Character parentCharacter) : base(4, parentCharacter, "GurenNoSouyoku")
		{
			Type = EffectType.Positive;
		}
		public override string GetDescription()
		{
			return $"{ParentCharacter.Name} rozwija skrzydła dzięki którym może poruszyć się o 3 pola więcej, ponadto może przelatywać przez ściany\n" +
						 "Czas do zakończenia efektu: " + CurrentCooldown;
		}
	}
}
