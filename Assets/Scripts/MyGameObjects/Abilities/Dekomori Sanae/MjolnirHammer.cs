using System;
using System.Collections.Generic;
using Helpers;
using Hex;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Dekomori_Sanae
{
	public class MjolnirHammer : Ability
	{
		private const int AbilityDamage = 18;
		private const int AbilityRange = 7;
		private bool WasUsedOnceThisTurn { get; set; }
		private Character FirstAbilityTarget { get; set; }
		public MjolnirHammer()
		{
			Name = "Mjolnir Hammer";
			Cooldown = 4;
			CurrentCooldown = 0;
			Type = AbilityType.Normal;
			WasUsedOnceThisTurn = false;
			FirstAbilityTarget = null;
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
		protected override void Use()
		{
			List<HexCell> cellRange = GetRangeCells();
			cellRange.RemoveNonEnemies();
			var canUseAbility = Active.Prepare(this, cellRange);
			if (canUseAbility) return;

			if (WasUsedOnceThisTurn)
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
			if (FirstAbilityTarget == targetCharacter)
			{
				damageToDeal /= 2;
			}
			var damage = new Damage(damageToDeal, DamageType.Physical);
			ParentCharacter.Attack(targetCharacter, damage);
			if (!WasUsedOnceThisTurn)
			{
				WasUsedOnceThisTurn = true;
				FirstAbilityTarget = targetCharacter;
				Use();
				return;
			}

			OnUseFinish();
		}
		public override void OnUseFinish()
		{
			base.OnUseFinish();
			WasUsedOnceThisTurn = false;
			FirstAbilityTarget = null;
		}
		public override void Cancel()
		{
			if (WasUsedOnceThisTurn)
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
