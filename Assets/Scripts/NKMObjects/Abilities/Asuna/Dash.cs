using System;
using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Asuna
{
	public class Dash : Ability
	{
		private const int AbilityRange = 4;
		private const int AbilityHitRange = 4;
		private const int AbilityCriticalHitRange = 2;
		private const int AbilityCriticalHitModifier = 3;

		private bool _hasDashed;

		public Dash()
		{
			Name = "Dash";
			Cooldown = 2;
			CurrentCooldown = 0;
			Type = AbilityType.Normal;
		}
		public override string GetDescription()
		{
			return string.Format(
@"{0} dashuje maksymalnie o {1} pola w linii prostej.
Jeżeli będzie w zasięgu {2} od przeciwnika w linii prostej, może go zaatakować,
a jeżeli będzie dodatkowo w zasięgu {3} od przeciwnika, atak ten zada {4}% obrażeń.
Czas odnowienia: {5} z atakiem, {6} bez ataku.",
ParentCharacter.Name, AbilityRange, AbilityHitRange, AbilityCriticalHitRange, AbilityCriticalHitModifier * 100, Cooldown + 1, Cooldown);
		}

		public override List<HexCell> GetRangeCells()
		{
//			List<HexCell> cellRange = ParentCharacter.ParentCell.GetNeighbors(AbilityRange, true, true, true);
			List<HexCell> cellRange = ParentCharacter.ParentCell.GetNeighbors(AbilityRange, SearchFlags.StopAtEnemyCharacters | SearchFlags.StopAtWalls | SearchFlags.StraightLine);
			return cellRange;
		}

		protected override void CheckIfCanBePrepared()
		{
			base.CheckIfCanBePrepared();
			List<HexCell> cellRange = GetRangeCells();
			cellRange.RemoveAll(c => c.CharacterOnCell != null);
			if (cellRange.Count == 0)
			{
				throw new Exception("Nie ma gdzie się ruszyć!");
			}
		}
		protected override void Use()
		{
			List<HexCell> cellRange = GetRangeCells();
			cellRange.RemoveAll(c => c.CharacterOnCell != null);
			var canUseAbility = Active.Prepare(this, cellRange);
			if (canUseAbility) return;

			MessageLogger.DebugLog("Nie ma nikogo w zasięgu umiejętności!");
			OnFailedUseFinish();
		}

		public override void Use(HexCell cell)
		{
			ParentCharacter.MoveTo(cell);
			_hasDashed = true;
//			List<HexCell> cellRange = ParentCharacter.ParentCell.GetNeighbors(AbilityHitRange, true, false, true);
			List<HexCell> cellRange = ParentCharacter.ParentCell.GetNeighbors(AbilityHitRange, SearchFlags.StopAtWalls | SearchFlags.StraightLine);
			cellRange.RemoveNonEnemies();
			var canUseAbility = Active.Prepare(this, cellRange);
			if (canUseAbility) return;

			MessageLogger.DebugLog("Nie ma nikogo w zasięgu umiejętności!");
			OnUseFinish();
		}

		public override void Use(Character targetCharacter)
		{
			var modifier = 1;
			if (ParentCharacter.ParentCell.GetNeighbors(2).Contains(targetCharacter.ParentCell)) modifier = AbilityCriticalHitModifier;
			var damageValue = ParentCharacter.AttackPoints.Value * modifier;
			var damage = new Damage(damageValue, DamageType.Physical);
			
			ParentCharacter.Attack(targetCharacter, damage);
			
			OnUseFinish(Cooldown+1);
		}

		public override void OnUseFinish()
		{
			base.OnUseFinish();
			_hasDashed = false;
		}

		public override void Cancel()
		{
			if(_hasDashed) OnUseFinish();
			else OnFailedUseFinish();
		}
	}
}
