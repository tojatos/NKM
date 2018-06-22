using System;
using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Rem
{
	public class MorgensternHit : Ability
	{
		private const int AbilityDamage = 15;
		private const int AbilityRange = 4;
		public MorgensternHit()
		{
			Name = "Morgenstern Hit";
			Cooldown = 4;
			CurrentCooldown = 0;
			Type = AbilityType.Normal;
		}
		protected override void CheckIfCanBePrepared()
		{
			base.CheckIfCanBePrepared();
			List<HexCell> cellRange = GetRangeCells();
			cellRange.RemoveNonEnemies();
			if (cellRange.Count == 0)
			{
				throw new Exception("Nie ma nikogo w zasięgu umiejętności!");
			}
		}

		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(AbilityRange);

		public override string GetDescription() => 
$@"{ParentCharacter.Name} wymachuje morgenszternem wokół własnej osi,
zadając wszystkim przeciwnikom w promieniu {AbilityRange} {AbilityDamage} obrażeń fizycznych.
Czas odnowienia: {Cooldown}";

		protected override void Use()
		{
			List<HexCell> cellRange = GetRangeCells();
			Active.Prepare(this, cellRange);
			Active.MakeAction(cellRange);

		}
		public override void Use(List<HexCell> cells)
		{
			cells.RemoveNonEnemies();
			List<Character> characters = cells.GetCharacters();
			characters.ForEach(c =>
			{
                var damage = new Damage(AbilityDamage, DamageType.Physical);
				ParentCharacter.Attack(c, damage);
			});
			OnUseFinish();
		}
	}
}
