using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities
{
	public class Empty : Ability
	{
		public Empty(AbilityType type)
		{
			Name = "Pusta umiejętność";
			Cooldown = 0;
			CurrentCooldown = 0;
			Type = type;
		}
		public override string GetDescription()
		{
			return "Twojego bohatera najwyraźniej nie stać na lepszą umiejętność.";
		}
		protected override void Use()
		{

			MessageLogger.DebugLog(ParentCharacter.Name + " spina poślady i... puf! Nic się nie dzieje.");
			OnFailedUseFinish();
		}

		//public override void Use(Character range)
		//{
		//	base.Use(range);

		//	MessageLogger.DebugLog("Gratulacje, próbujesz użyć niczego.");

		//	OnUseFinish();
		//}

	}
}
