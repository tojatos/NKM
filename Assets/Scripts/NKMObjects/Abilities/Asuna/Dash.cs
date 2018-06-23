using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Asuna
{
	public class Dash : Ability, IClickable
	{
		private const int AbilityRange = 4;
		private const int AbilityHitRange = 4;
		private const int AbilityCriticalHitRange = 2;
		private const int AbilityCriticalHitModifier = 3;

		private bool _hasDashed;

		public Dash() : base(AbilityType.Normal, "Dash", 2)
		{
			OnAwake += () => Validator.ToCheck.Add(IsThereACellToMove);
		}
		
		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(AbilityRange, SearchFlags.StopAtEnemyCharacters | SearchFlags.StopAtWalls | SearchFlags.StraightLine);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().FindAll(c => c.CharacterOnCell == null);
		
		public override string GetDescription() => 
$@"{ParentCharacter.Name} dashuje maksymalnie o {AbilityRange} pola w linii prostej.
Jeżeli będzie w zasięgu {AbilityHitRange} od przeciwnika w linii prostej, może go zaatakować,
a jeżeli będzie dodatkowo w zasięgu {AbilityCriticalHitRange} od przeciwnika, atak ten zada {AbilityCriticalHitModifier * 100}% obrażeń.
Czas odnowienia: {Cooldown + 1} z atakiem, {Cooldown} bez ataku.";

//		protected override void CheckIfCanBePrepared()
//		{
//			base.CheckIfCanBePrepared();
//			List<HexCell> cellRange = GetRangeCells();
//			cellRange.RemoveAll(c => c.CharacterOnCell != null);
//			if (cellRange.Count == 0)
//			{
//				throw new Exception("Nie ma gdzie się ruszyć!");
//			}
//		}

		private bool IsThereACellToMove() => GetRangeCells().Any(c => c.CharacterOnCell == null);

		public void Click()
		{
//			List<HexCell> cellRange = GetRangeCells();
//			cellRange.RemoveAll(c => c.CharacterOnCell != null);
//			var canUseAbility = Active.Prepare(this, cellRange);
//			if (canUseAbility) return;

//			MessageLogger.DebugLog("Nie ma nikogo w zasięgu umiejętności!");
//			OnFailedUseFinish();
			Active.Prepare(this, GetTargetsInRange());
		}

		public override void Use(HexCell cell)
		{
			ParentCharacter.MoveTo(cell);
			_hasDashed = true;
//			List<HexCell> cellRange = ParentCharacter.ParentCell.GetNeighbors(AbilityHitRange, true, false, true);
			List<HexCell> cellRange = ParentCharacter.ParentCell.GetNeighbors(AbilityHitRange, SearchFlags.StopAtWalls | SearchFlags.StraightLine).WhereOnlyEnemies();
//			cellRange.RemoveNonEnemies();
			if(cellRange.Count > 0) Active.Prepare(this, cellRange);
//			var canUseAbility = Active.Prepare(this, cellRange);
//			if (canUseAbility) return;

//			MessageLogger.DebugLog("Nie ma nikogo w zasięgu umiejętności!");
//			OnUseFinish();
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
