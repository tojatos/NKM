using System;
using System.Collections.Generic;
using Helpers;
using Hex;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Asuna
{
	public class Dash : Ability
	{
		private const int AbilityRange = 4;
		private const int AbilityHitRange = 4;
		private const int AbilityCriticalHitRange = 2;
		private const int AbilityCriticalHitModifier = 3;

		private bool HasDashed;

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
			var cellRange = ParentCharacter.ParentCell.GetNeighbors(AbilityRange, true, true, true);
			return cellRange;
		}

		protected override void CheckIfCanBePrepared()
		{
			base.CheckIfCanBePrepared();
			var cellRange = GetRangeCells();
			cellRange.RemoveAll(c => c.CharacterOnCell != null);
			if (cellRange.Count == 0)
			{
				throw new Exception("Nie ma gdzie się ruszyć!");
			}
		}
		protected override void Use()
		{
			var cellRange = GetRangeCells();
			cellRange.RemoveAll(c => c.CharacterOnCell != null);
			var canUseAbility = Active.Prepare(this, cellRange);
			if (canUseAbility) return;

			MessageLogger.DebugLog("Nie ma nikogo w zasięgu umiejętności!");
			OnFailedUseFinish();
		}

		public override void Use(HexCell cell)
		{
			ParentCharacter.MoveTo(cell);
			HasDashed = true;
			var cellRange = ParentCharacter.ParentCell.GetNeighbors(AbilityHitRange, true, false, true);
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
			ParentCharacter.Attack(targetCharacter, AttackType.Physical, ParentCharacter.AttackPoints.Value * modifier);
			OnUseFinish(Cooldown+1);
		}

		public override void OnUseFinish()
		{
			base.OnUseFinish();
			HasDashed = false;
		}

		public override void Cancel()
		{
			if(HasDashed) OnUseFinish();
			else OnFailedUseFinish();
		}
	}
}
