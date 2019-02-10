using System;
using System.Collections.Generic;
using System.Linq;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;
using Unity;

namespace NKMCore.Abilities.Itsuka_Kotori
{
    public class CamaelMegiddo : Ability, IClickable, IUseableCell
    {
        private const int LineDamage = 40;
        private const int ConflargationDamage = 20;

        public CamaelMegiddo(Game game) : base(game, AbilityType.Ultimatum, "Camael - Megiddo", 6) { }

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
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner);

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
            if (!hitConflargation) return cells;
            List<HexCell> conflargationCells = lastCell.GetNeighbors(Owner.Owner, 500000, SearchFlags.None, neighbor => //TODO wtf
                !neighbor.Effects.ContainsType<HexCellEffects.Conflagration>());
            cells.AddRange(conflargationCells);
            //TODO: find a better value than 500000
            return cells;
        }

        public void Click()
        {
            List<HexCell> cellRange = GetNeighboursOfOwner(1);
            Active.Prepare(this, cellRange); //TODO: Air selection magic? Or maybe new mechanism?
        }

        private void SendFlamewave(HexDirection direction)
        {
            List<HexCell> cells = GetDirectionRangeCells(direction);
            IGrouping<bool, HexCell>[] cellsByConflargation = cells.GroupBy(c => c.Effects.ContainsType<HexCellEffects.Conflagration>()).ToArray();
			List<HexCell> lineCells = cellsByConflargation.Where(k => k.Key == false).SelectMany(x => x).ToList();
			List<HexCell> conflargationCells = cellsByConflargation.Where(k => k.Key).SelectMany(x => x).ToList();
            AnimationPlayer.Add(new Unity.Animations.CamaelMegiddo(lineCells.Select(c => Active.SelectDrawnCell(c).transform).ToList(), conflargationCells.Select(c => Active.SelectDrawnCell(c).transform).ToList()));

			//deal damages
			lineCells.WhereEnemiesOf(Owner).GetCharacters().ForEach(c => ParentCharacter.Attack(this, c, new Damage(LineDamage, DamageType.Magical)));
			conflargationCells.WhereEnemiesOf(Owner).GetCharacters().ForEach(c => ParentCharacter.Attack(this, c, new Damage(ConflargationDamage, DamageType.Magical)));

			//remove conflargation effects
//			conflargationCells.ForEach(c => c.Effects.RemoveAll(e => e.GetType() == typeof(HexCellEffects.Conflagration)));
			conflargationCells.ForEach(c => c.Effects.FindAll(e => e.GetType() == typeof(HexCellEffects.Conflagration)).ForEach(e => e.Remove()));

        }

        public void Use(HexCell cell)
        {
            ParentCharacter.TryToTakeTurn();
            HexDirection direction = ParentCharacter.ParentCell.GetDirection(cell);
            SendFlamewave(direction);
            Finish();
        }
    }
}
