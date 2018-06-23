using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Rem
{
	public class MorgensternHit : Ability, IClickable
	{
		private const int AbilityDamage = 15;
		private const int AbilityRange = 4;

		public MorgensternHit() : base(AbilityType.Normal, "Morgenstern Hit", 4)
		{
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
		}
		
		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(AbilityRange);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemies();

		public override string GetDescription() => 
$@"{ParentCharacter.Name} wymachuje morgenszternem wokół własnej osi,
zadając wszystkim przeciwnikom w promieniu {AbilityRange} {AbilityDamage} obrażeń fizycznych.
Czas odnowienia: {Cooldown}";

		public void Click()
		{
			Active.Prepare(this, GetTargetsInRange());
			Active.MakeAction(Active.HexCells);
		}
		public override void Use(List<HexCell> cells)
		{
//			cells.RemoveNonEnemies();
//			List<Character> characters = cells.GetCharacters();
//			characters.ForEach(c =>
			cells.GetCharacters().ForEach(c => ParentCharacter.Attack(c, new Damage(AbilityDamage, DamageType.Physical)));
			OnUseFinish();
		}
	}
}
