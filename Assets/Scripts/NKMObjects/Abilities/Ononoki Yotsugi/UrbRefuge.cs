using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Ononoki_Yotsugi
{
    public class UrbRefuge : Ability, IClickable, IUseableCell
    {
        private const int Damage = 20;
        private const int CharacterGrabRange = 5;
        private const int Range = 14;
        private const int Radius = 4;
        public UrbRefuge(Game game) : base(game, AbilityType.Ultimatum, "URB - Refuge", 7)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override string GetDescription() =>
            $@"{ParentCharacter.FirstName()} zabiera ze sobą wskazanego sojusznika i przeskakuje we wskazany obszar o promieniu {Radius}.
Wszyscy wrogowie, jak i zabrany sojusznik otrzymują {Damage} obrażeń fizycznych.

Zasięg zabrania sojusznika: {CharacterGrabRange}
Zasięg skoku: {Range}
Czas odnowienia; {Cooldown}";

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(CharacterGrabRange);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereFriendsOf(Owner);

        private List<HexCell> GetTargetCells() => GetNeighboursOfOwner(Range)
            .FindAll(c => c.IsFreeToStand && c.GetNeighbors(Owner.Owner, Radius).Any(ce => ce.IsFreeToStand));

        public void Click() => Active.Prepare(this, GetTargetsInRange());

        private Character _characterToTake;
        private HexCell _targetCell;
        public void Use(HexCell cell)
        {
            if (!cell.IsEmpty)
            {
                _characterToTake = cell.FirstCharacter;
                Active.Prepare(this, GetTargetCells());
            }
            else if (_targetCell == null)
            {
                _targetCell = cell;
                Active.Prepare(this, _targetCell.GetNeighbors(Owner.Owner, Radius).FindAll(c => c.IsFreeToStand));
            }
            else
            {
                ParentCharacter.TryToTakeTurn();
                ParentCharacter.MoveTo(_targetCell);
                _characterToTake.MoveTo(cell);

                ParentCharacter.Attack(this, _characterToTake, new Damage(Damage, DamageType.Physical));
                _targetCell.GetNeighbors(Owner.Owner, Radius).WhereEnemiesOf(Owner).GetCharacters()
                    .ForEach(c => ParentCharacter.Attack(this, c, new Damage(Damage, DamageType.Physical)));

                _targetCell = null;
                Finish();
            }
        }
    }
}