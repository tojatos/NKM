using System;
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
		private bool _wasUsedOnceThisTurn;// { get; set; }
		private Character _firstAbilityTarget;// { get; set; }
		public MjolnirHammer() : base(AbilityType.Normal, "Mjolnir Hammer", 4)
		{
//			Name = "Mjolnir Hammer";
//			Cooldown = 4;
//			CurrentCooldown = 0;
//			Type = AbilityType.Normal;
//			WasUsedOnceThisTurn = false;
//			FirstAbilityTarget = null;
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
@"{0} uderza dwukrotnie, zadając {1} obrażeń fizycznych przy każdym ciosie.
Jeżeli obydwa ataki wymierzone są w ten sam cel, otrzymuje on połowę obrażeń od drugiego uderzenia.
Zasięg: {2}	Czas odnowienia: {3}",
ParentCharacter.Name, AbilityDamage, AbilityRange, Cooldown);
		}

		public void ImageClick()
		{
			List<HexCell> cellRange = GetRangeCells();
			cellRange.RemoveNonEnemies();
			var canUseAbility = Active.Prepare(this, cellRange);
			if (canUseAbility) return;

			if (_wasUsedOnceThisTurn)
			{
				OnUseFinish();
			}
			else
			{
				MessageLogger.DebugLog("Nie ma nikogo w zasięgu umiejętności!");
				OnFailedUseFinish();
			}
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
				ImageClick();
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
			if (_wasUsedOnceThisTurn)
			{
				OnUseFinish();
			}
			else
			{
				OnFailedUseFinish();
			}
		}
	}
}
