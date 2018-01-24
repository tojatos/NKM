using System.Collections.Generic;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Yasaka_Mahiro
{
	public class WhenTheyCry : Ability
	{
		private const int AdditionalDamagePercent = 25;
		private readonly List<Character> DamagedCharacters;
		public WhenTheyCry()
		{
			Name = "When They Cry";
			Type = AbilityType.Passive;
			DamagedCharacters = new List<Character>();
		}
		public override string GetDescription()
		{
			return $"{ParentCharacter.Name} zadaje dodatkowe {AdditionalDamagePercent}% obrażeń zranionym wcześniej wrogom.";
		}
		public override void DamageModifier(Character targetCharacter, ref int damage)
		{
			if (DamagedCharacters.Contains(targetCharacter))
			{
				damage += damage * AdditionalDamagePercent / 100;
			}
		}

		public override void OnDamage(Character targetCharacter, int damageDealt)
		{
			if(targetCharacter.Owner!=Active.GamePlayer&&!DamagedCharacters.Contains(targetCharacter)) DamagedCharacters.Add(targetCharacter);
		}
	}
}
