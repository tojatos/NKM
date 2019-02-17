﻿using System.Collections.Generic;
using System.Linq;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Asuna
{
	public class Dash : Ability, IClickable, IUseableCell
	{
		private const int AbilityRange = 4;
		private const int AbilityHitRange = 4;
		private const int AbilityCriticalHitRange = 2;
		private const int AbilityCriticalHitModifier = 3;

		private bool _hasDashed;

		public Dash(Game game) : base(game, AbilityType.Normal, "Dash", 2)
		{
			OnAwake += () => Validator.ToCheck.Add(IsThereACellToMove);
			AfterUseFinish += () => _hasDashed = false;
			CanUseOnGround = false;
		}
		
		public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(AbilityRange, SearchFlags.StopAtEnemyCharacters | SearchFlags.StopAtWalls | SearchFlags.StraightLine);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().FindAll(c => c.IsFreeToStand);
		
		public override string GetDescription() => 
$@"{ParentCharacter.Name} dashuje maksymalnie o {AbilityRange} pola w linii prostej.
Jeżeli będzie w zasięgu {AbilityHitRange} od przeciwnika w linii prostej, może go zaatakować,
a jeżeli będzie dodatkowo w zasięgu {AbilityCriticalHitRange} od przeciwnika, atak ten zada {AbilityCriticalHitModifier * 100}% obrażeń.
Czas odnowienia: {Cooldown + 1} z atakiem, {Cooldown} bez ataku.";
		
		private bool IsThereACellToMove() => GetRangeCells().Any(c => c.IsFreeToStand);

		public void Click() => Active.Prepare(this, GetTargetsInRange());
		private void DashTo(HexCell cell)
		{
			ParentCharacter.TryToTakeTurn();
			ParentCharacter.MoveTo(cell);
			_hasDashed = true;
			List<HexCell> cellRange = GetNeighboursOfOwner(AbilityHitRange, SearchFlags.StopAtWalls | SearchFlags.StraightLine).WhereEnemiesOf(Owner);
			if(cellRange.Count > 0) Active.Prepare(this, cellRange);
			else Finish();
		}
		private void AfterDashAttack(Character targetCharacter)
		{
			int modifier = 1;
			if (GetNeighboursOfOwner(AbilityCriticalHitRange).Contains(targetCharacter.ParentCell)) modifier = AbilityCriticalHitModifier;
			int damageValue = ParentCharacter.AttackPoints.Value * modifier;
			var damage = new Damage(damageValue, DamageType.Physical);
			
			ParentCharacter.Attack(this, targetCharacter, damage);
			
			Finish(Cooldown+1);
		}

		public override void Cancel()
		{
			if(_hasDashed) Finish();
			else OnFailedUseFinish();
		}

		public void Use(HexCell cell)
		{
			if(!_hasDashed) DashTo(cell);
			else AfterDashAttack(cell.FirstCharacter);
		}
	}
}
