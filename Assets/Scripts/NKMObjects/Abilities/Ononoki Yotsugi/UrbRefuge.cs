using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Ononoki_Yotsugi
{
    public class UrbRefuge : Ability, IClickable, IUseable
    {
        private const int Damage = 20;
        private const int CharacterGrabRange = 5;
        private const int Range = 14;
        private const int Radius = 4;
        public UrbRefuge() : base(AbilityType.Ultimatum, "URB - Refuge", 7)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override string GetDescription() =>
            $@"{ParentCharacter.FirstName()} zabiera ze sobą wskazanego sojusznika i przeskakuje we wskazany obszar o promieniu {Radius}.
Wszyscy wrogowie, jak i zabrany sojusznik otrzymują {Damage} obrażeń fizycznych.

Zasięg zabrania sojusznika: {CharacterGrabRange}
Zasięg skoku: {Range}
Czas odnowienia; {Cooldown}";

        public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(CharacterGrabRange);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyFriendsOf(Owner);

        private List<HexCell> GetTargetCells() => ParentCharacter.ParentCell.GetNeighbors(Range)
            .FindAll(c => c.IsFreeToStand && c.GetNeighbors(Radius).Any(ce => ce.IsFreeToStand));

        public void Click() => Active.Prepare(this, GetTargetsInRange());

        private Character _characterToTake;
        private HexCell _targetCell;
        public void Use(List<HexCell> cells)
        {
            HexCell targetCell = cells[0];
            if (targetCell.CharacterOnCell != null)
            {
                _characterToTake = targetCell.CharacterOnCell;
                Active.Prepare(this, GetTargetCells());
            }
            else if (_targetCell == null)
            {
                _targetCell = targetCell;
                Active.Prepare(this, _targetCell.GetNeighbors(Radius).FindAll(c => c.IsFreeToStand));
            }
            else
            {
                ParentCharacter.MoveTo(_targetCell);
                _characterToTake.MoveTo(targetCell);

                ParentCharacter.Attack(this, _characterToTake, new Damage(Damage, DamageType.Physical));
                _targetCell.GetNeighbors(Radius).WhereOnlyEnemiesOf(Owner).GetCharacters()
                    .ForEach(c => ParentCharacter.Attack(this, c, new Damage(Damage, DamageType.Physical)));

                _targetCell = null;
                Finish();
            }
        }
    }
}