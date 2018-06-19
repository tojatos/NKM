﻿using System;
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

		public override List<HexCell> GetRangeCells()
		{
			return ParentCharacter.ParentCell.GetNeighbors(AbilityRange);
		}

		public override string GetDescription()
		{
			return string.Format(
@"{0} wymachuje morgenszternem wokół własnej osi zadając wszystkim przeciwnikom w promieniu {1},
Zadając {2} obrażeń fizycznych.
Czas odnowienia: {3}",
				ParentCharacter.Name, AbilityRange, AbilityDamage, Cooldown);
		}
		protected override void Use()
		{
			List<HexCell> cellRange = GetRangeCells();
			Active.Prepare(this, cellRange);
			Active.MakeAction(cellRange);

		}
		public override void Use(List<HexCell> cells)
		{
			List<Character> characters = cells.GetCharacters();
			var damage = new Damage(AbilityDamage, DamageType.Physical);
			characters.ForEach(c =>ParentCharacter.Attack(c, damage));
			OnUseFinish();
		}
	}
}
