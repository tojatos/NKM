using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Dekomori_Sanae
{
	public class MjolnirHammer : Ability, IClickable
	{
		private const int AbilityDamage = 18;
		private const int AbilityRange = 7;
		private bool _wasUsedOnceThisTurn; // { get; set; }
		private Character _firstAbilityTarget; // { get; set; }

		public MjolnirHammer() : base(AbilityType.Normal, "Mjolnir Hammer", 4)
		{
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
		}

		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(AbilityRange);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);

		public override string GetDescription() =>
			$@"{ParentCharacter.Name} uderza dwukrotnie, zadając {AbilityDamage} obrażeń fizycznych przy każdym ciosie.
Jeżeli obydwa ataki wymierzone są w ten sam cel, otrzymuje on połowę obrażeń od drugiego uderzenia.
Zasięg: {AbilityRange}	Czas odnowienia: {Cooldown}";

		public void Click()
		{
//			List<HexCell> cellRange = GetRangeCells();
//			cellRange.RemoveNonEnemies();
//			var canUseAbility = Active.Prepare(this, cellRange);
//			if (canUseAbility) return;
			PrepareHammerHit();

		}

		private void PrepareHammerHit()
		{
			if (!CanBeUsed) Cancel();
			else Active.Prepare(this, GetTargetsInRange());
		}
		

	public override void Use(Character targetCharacter)
		{
			var damageToDeal = AbilityDamage;
			if (_firstAbilityTarget == targetCharacter)
			{
				damageToDeal /= 2;
			}
			var damage = new Damage(damageToDeal, DamageType.Physical);
			ParentCharacter.Attack(targetCharacter, damage);
			if (!_wasUsedOnceThisTurn)
			{
				_wasUsedOnceThisTurn = true;
				_firstAbilityTarget = targetCharacter;
				Click();
				return;
			}

			OnUseFinish();
		}
		public override void OnUseFinish()
		{
			base.OnUseFinish();
			_wasUsedOnceThisTurn = false;
			_firstAbilityTarget = null;
		}
		public override void Cancel()
		{
            if (_wasUsedOnceThisTurn) OnUseFinish();
            else OnFailedUseFinish();
		}
	}
}
