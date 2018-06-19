using NKMObjects.Templates;

namespace NKMObjects.Abilities.Aqua
{
	public class NaturesBeauty : Ability
	{
		public NaturesBeauty()
		{
			Name = "Nature's Beauty";
			Type = AbilityType.Passive;
//			OverridesFriendAttack = true;
		}

		public override string GetDescription() => $"{ParentCharacter.Name} może używać podstawowych ataków na sojuszników, lecząc ich za ilość HP równą jej obecnemu atakowi.";

//		public override void AttackFriend(Character attackedCharacter, int damage) => ParentCharacter.Heal(attackedCharacter, damage);
		public override void Awake()
		{
			ParentCharacter.CanAttackAllies = true;
			ParentCharacter.BasicAttack = character =>
			{
				if (character.Owner == ParentCharacter.Owner)
				{
					ParentCharacter.Heal(character, ParentCharacter.AttackPoints.Value);
				}
				else ParentCharacter.DefaultBasicAttack(character);

			};
		}
	}
}
