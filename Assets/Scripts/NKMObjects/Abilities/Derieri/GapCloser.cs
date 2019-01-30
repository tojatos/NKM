using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Derieri
{
    public class GapCloser : Ability, IClickable, IUseableCharacter, IUseableCell
    {
        private const int MaxDistanceFromTarget = 2;
        private const int Range = 9;
        public GapCloser(Game game) : base(game, AbilityType.Ultimatum, "Gap closer", 4)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
            CanUseOnGround = false;
        }

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} przemieszcza się w pole oddalone max. {MaxDistanceFromTarget} jednostki od wcześniej zaatakowanego wroga.
Dodatkowo otrzymuje możliwość podstawowego ataku.

Zasięg: {Range}    Czas odnowienia: {Cooldown}";

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);
        public override List<HexCell> GetTargetsInRange()
        {
			ComboStar passiveAbility = ParentCharacter.Abilities.OfType<ComboStar>().SingleOrDefault();
            Character targetCharacter = passiveAbility?.ComboCharacter;
            return targetCharacter==null ? new List<HexCell>() : GetRangeCells().FindAll(c => c.FirstCharacter == targetCharacter).FindAll(c => c.GetNeighbors(Owner.Owner, MaxDistanceFromTarget).Any(cc => cc.IsFreeToStand));
        }

        public void Click() => Active.Prepare(this, GetTargetsInRange());

        public void Use(Character character) =>
            Active.Prepare(this, character.ParentCell.GetNeighbors(Owner.Owner, MaxDistanceFromTarget).FindAll(c => c.IsFreeToStand));

        public void Use(HexCell cell)
        {
            ParentCharacter.TryToTakeTurn();
            ParentCharacter.MoveTo(cell);
            ParentCharacter.HasFreeAttackUntilEndOfTheTurn = true;
            Finish();
        }
    }
}