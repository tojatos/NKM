using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Itsuka_Kotori
{
    public class Conflagration : Ability, IClickable, IUseable
    {
        private const int Range = 10;
        private const int Radius = 3;
        private const int DamagePercent = 50;

        public Conflagration(Game game) : base(game, AbilityType.Normal, "Conflagration", 2)
        {
            OnAwake += () =>
            {
                ParentCharacter.BeforeBasicAttack += (character, damage) =>
                {
                    bool isEnemyOnConflagration = character.ParentCell.Effects.ContainsType(typeof(HexCellEffects.Conflagration));
                    bool isEnemyInBasicAttackRange = ParentCharacter.DefaultGetBasicAttackCells().GetCharacters().Contains(character);
                    if (!isEnemyInBasicAttackRange && isEnemyOnConflagration) damage.Value = (int) (damage.Value * (DamagePercent / 100f));
                };
                ParentCharacter.GetBasicAttackCells = () =>
                {
                    List<HexCell> cellRange = ParentCharacter.DefaultGetBasicAttackCells();
                    IEnumerable<HexCell> cellsWithConflagrationAndEnemyCharacters = HexMap.Cells
                        .Where(c => c.Effects.Any(e => e.GetType() == typeof(HexCellEffects.Conflagration)))
                        .ToList().WhereEnemiesOf(Owner);
                    cellRange.AddRange(cellsWithConflagrationAndEnemyCharacters);
                    return cellRange.Distinct().ToList();
                };
            };
        }
       
        public override string GetDescription() => string.Format(
@"{0} wywołuje Pożar na wskazanym obszarze o promieniu {3}.
{0} może atakować wrogów znajdujących się na terenie Pożaru podstawowymi atakami, zadając 50% zwykłych obrażeń, niezależnie od tego gdzie sama się znajduje.

Zasięg: {1}	Czas odnowienia: {2}",
            ParentCharacter.Name, Range, Cooldown, Radius);

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);

        public void Click()
        {
            List<HexCell> cellRange = GetRangeCells();
            Active.Prepare(this, cellRange, false, false);
            Active.AirSelection.Enable(AirSelection.SelectionShape.Circle, Radius);
        }

        public void Use(List<HexCell> cells)
        {
            cells.ForEach(c => c.Effects.Add(new HexCellEffects.Conflagration(Game, -1, c, ParentCharacter)));
            Finish();
        }
    }
}
