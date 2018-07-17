using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Itsuka_Kotori
{
    public class CamaelMegiddo : Ability, IClickable, IUseable
    {
        private const int LineDamage = 40;
        private const int ConflargationDamage = 20;

        public CamaelMegiddo() : base(AbilityType.Ultimatum, "Camael - Megiddo", 6) { }

        public override string GetDescription() =>
$@"{ParentCharacter.Name} wystrzeliwuje falę płomieni w wybranym kierunku zadając {LineDamage} obrażeń magicznych wszystkim trafionym wrogom.
Jeżeli ta umiejętność uderzy w obszar Conflagration, zada ona {ConflargationDamage} na całym tym obszarze, ale nie poleci dalej.
Efekty Conflagration znikają po trafieniu tą umiejętnością.

Czas odnowienia: {Cooldown}";
        public override List<HexCell> GetRangeCells()
        {
            List<HexCell> cells = new List<HexCell>();
            foreach (HexDirection direction in Enum.GetValues(typeof(HexDirection)))
            {
                cells.AddRange(GetDirectionRangeCells(direction));
            }
            return cells;
        }
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);

        private List<HexCell> GetDirectionRangeCells(HexDirection direction)
        {
            List<HexCell> cells = new List<HexCell>();
            HexCell lastCell = ParentCharacter.ParentCell;
            bool hitConflargation = false;

            //first the line
            while (true)
            {
                HexCell neighbor = lastCell.GetCell(direction, 1);
                if (neighbor == null) break;
                if (neighbor.Effects.ContainsType(typeof(HexCellEffects.Conflagration)))
                {
                    hitConflargation = true;
                    break;
                }
                cells.Add(neighbor);
                lastCell = neighbor;
            }

            //then the conflargation cells
            if (hitConflargation)
            {
                var conflargationCells = lastCell.GetNeighbors(500000, SearchFlags.None, (HexCell neighbor) =>
                      ((!neighbor.Effects.ContainsType<HexCellEffects.Conflagration>()) ? true : false));
                cells.AddRange(conflargationCells);
                //TODO: find a better value than 500000
            }
            return cells;

        }

        public void Click()
        {
            List<HexCell> cellRange = ParentCharacter.ParentCell.GetNeighbors(1);
            Active.Prepare(this, cellRange); //TODO: Air selection magic? Or maybe new mechanism?
        }

        private void SendFlamewave(HexDirection direction)
        {
            List<HexCell> cells = GetDirectionRangeCells(direction);
            List<HexCell> lineCells;
            List<HexCell> conflargationCells;
            IEnumerable<IGrouping<bool, HexCell>> cellsByConflargation = cells.GroupBy(c => c.Effects.ContainsType<HexCellEffects.Conflagration>());
			lineCells = cellsByConflargation.Where(k => k.Key == false).SelectMany(x => x).ToList();
			conflargationCells = cellsByConflargation.Where(k => k.Key == true).SelectMany(x => x).ToList();
            AnimationPlayer.Add(new Animations.CamaelMegiddo(lineCells.Select(c => c.transform).ToList(), conflargationCells.Select(c => c.transform).ToList()));

			//deal damages
			lineCells.WhereOnlyEnemiesOf(Owner).GetCharacters().ForEach(c => ParentCharacter.Attack(this, c, new Damage(LineDamage, DamageType.Magical)));
			conflargationCells.WhereOnlyEnemiesOf(Owner).GetCharacters().ForEach(c => ParentCharacter.Attack(this, c, new Damage(ConflargationDamage, DamageType.Magical)));

			//remove conflargation effects
			conflargationCells.ForEach(c => c.Effects.RemoveAll(e => e.GetType() == typeof(HexCellEffects.Conflagration)));

        }

        public void Use(List<HexCell> cells)
        {
            HexDirection direction = ParentCharacter.ParentCell.GetDirection(cells[0]);
            SendFlamewave(direction);
            Finish();
        }
    }
}
