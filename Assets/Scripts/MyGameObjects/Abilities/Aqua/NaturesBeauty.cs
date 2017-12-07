using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Aqua
{
	public class NaturesBeauty : Ability
	{
		public NaturesBeauty()
		{
			Name = "Nature's Beauty";
			Type = AbilityType.Passive;
			OverridesFriendAttack = true;
		}

		public override string GetDescription() => $"{ParentCharacter.Name} może używać podstawowych ataków na sojuszników, lecząc ich za ilość HP równą jej obecnemu atakowi.";

		public override void AttackFriend(Character attackedCharacter) => ParentCharacter.Heal(attackedCharacter, ParentCharacter.AttackPoints.Value);
	}
}
