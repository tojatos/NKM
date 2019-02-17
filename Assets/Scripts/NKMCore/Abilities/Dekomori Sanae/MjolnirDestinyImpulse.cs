using System.Collections.Generic;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Dekomori_Sanae
{
	public class MjolnirDestinyImpulse : Ability, IClickable, IUseableCellList
	{
		private const int Damage = 25;
		private const int Range = 8;
		private bool _wasUsedOnceThisTurn;
		public MjolnirDestinyImpulse(Game game) : base(game, AbilityType.Ultimatum, "Mjolnir Destiny Impulse", 6)
		{
			AfterUseFinish += () => _wasUsedOnceThisTurn = false;
		}
		public override string GetDescription() => 
$@"{ParentCharacter.Name} uderza swoim młotem w wybrany obszar,
zadając {Damage} obrażeń fizycznych wszystkim wrogom na tym terenie.
Jeżeli {ParentCharacter.Name} zabije chociaż jedną postać za pomocą tej umiejętności,
może ona użyć tej umiejętności ponownie, w tej samej turze.

Zasięg: {Range}	Czas odnowienia: {Cooldown}";

		public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);

		public void Click() => PrepareImpulse();
		private void PrepareImpulse()
		{
			List<HexCell> cellRange = GetRangeCells();
			cellRange.Add(ParentCharacter.ParentCell);
			Active.PrepareAirSelection(this, cellRange, AirSelection.SelectionShape.Circle, 1);
		}
		public void Use(List<HexCell> cells)
		{
            ParentCharacter.TryToTakeTurn();
			List<Character> characters = cells.GetCharacters();
			bool killedSomeone = false;
			characters.ForEach(targetCharacter =>
			{
				if (targetCharacter.Owner == Active.GamePlayer) return;
				
				var damage = new Damage(Damage, DamageType.Physical);

				ParentCharacter.Attack(this,targetCharacter, damage);
				_wasUsedOnceThisTurn = true;
				if (!targetCharacter.IsAlive)
				{
					killedSomeone = true;
				}
			});
			if (killedSomeone) PrepareImpulse();
			else Finish();
		}
		public override void Cancel()
		{
			if (_wasUsedOnceThisTurn) Finish();
			else OnFailedUseFinish();
		}
	}
}
