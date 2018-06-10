using System.Collections.Generic;
using Helpers;
using Hex;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Dekomori_Sanae
{
	public class MjolnirDestinyImpulse : Ability
	{
		private const int AbilityDamage = 25;
		private const int AbilityRange = 8;
		private bool _wasUsedOnceThisTurn;
		public MjolnirDestinyImpulse()
		{
			Name = "Mjolnir Destiny Impulse";
			Cooldown = 6;
			CurrentCooldown = 0;
			Type = AbilityType.Ultimatum;
		}
		public override string GetDescription()
		{
			return string.Format(
@"{0} uderza swoim młotem w wybrany obszar,
zadając {1} obrażeń fizycznych wszystkim wrogom na tym terenie.
Jeżeli {0} zabije chociaż jedną postać za pomocą tej umiejętności,
może ona użyć tej umiejętności ponownie, w tej samej turze.
Zasięg: {2}	Czas odnowienia: {3}",
				ParentCharacter.Name, AbilityDamage, AbilityRange, Cooldown);
		}

		public override List<HexCell> GetRangeCells()
		{
			return ParentCharacter.ParentCell.GetNeighbors(AbilityRange);
		}

		protected override void Use()
		{
			List<HexCell> cellRange = GetRangeCells();
			cellRange.Add(ParentCharacter.ParentCell);
			Active.Prepare(this, cellRange, false, false);
			Active.AirSelection.Enable(AirSelection.SelectionShape.Circle, 1);
		}
		public override void Use(List<HexCell> cells)
		{
			List<Character> characters = cells.GetCharacters();
			var killedSomething = false;
			characters.ForEach(targetCharacter =>
			{
				if (targetCharacter.Owner == Active.GamePlayer) return;
				
				var damage = new Damage(AbilityDamage, DamageType.Physical);

				ParentCharacter.Attack(targetCharacter, damage);
				_wasUsedOnceThisTurn = true;
				if (!targetCharacter.IsAlive)
				{
					killedSomething = true;
				}
			});
			if (killedSomething) Use();
			else OnUseFinish();
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
		public override void OnUseFinish()
		{
			base.OnUseFinish();
			_wasUsedOnceThisTurn = false;
		}
	}
}
