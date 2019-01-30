using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Dekomori_Sanae
{
	public class MjolnirHammer : Ability, IClickable, IUseableCharacter
	{
		private const int Damage = 18;
		private const int Range = 7;
		private bool _wasUsedOnceThisTurn;
		private Character _firstAbilityTarget;

		public MjolnirHammer(Game game) : base(game, AbilityType.Normal, "Mjolnir Hammer", 4)
		{
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
			AfterUseFinish += () =>
			{
				_wasUsedOnceThisTurn = false;
				_firstAbilityTarget = null;
			};
		}

		public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner);

		public override string GetDescription() =>
$@"{ParentCharacter.Name} uderza dwukrotnie, zadając {Damage} obrażeń fizycznych przy każdym ciosie.
Jeżeli obydwa ataki wymierzone są w ten sam cel, otrzymuje on połowę obrażeń od drugiego uderzenia.
Zasięg: {Range}	Czas odnowienia: {Cooldown}";

		public void Click() => PrepareHammerHit();

		private void PrepareHammerHit()
		{
			if (!CanBeUsed) Cancel();
			else Active.Prepare(this, GetTargetsInRange());
		}
		
		public void Use(Character targetCharacter)
		{
            ParentCharacter.TryToTakeTurn();
			int damageToDeal = Damage;
			if (_firstAbilityTarget == targetCharacter)
			{
				damageToDeal /= 2;
			}
			var damage = new Damage(damageToDeal, DamageType.Physical);
			ParentCharacter.Attack(this,targetCharacter, damage);
			if (!_wasUsedOnceThisTurn)
			{
				_wasUsedOnceThisTurn = true;
				_firstAbilityTarget = targetCharacter;
				Click();
				return;
			}

			Finish();
		}
		public override void Cancel()
		{
            if (_wasUsedOnceThisTurn) Finish();
            else OnFailedUseFinish();
		}
	}
}
