using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using Hex;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Itsuka_Kotori
{
    public class Conflagration : Ability
    {
        private const int Range = 13;
        private const int Radius = 6;
        private const int EffectTime = 3;
        private const int AdditionalDamagePercent = 50;

        public Conflagration()
        {
            Name = "Conflagration";
            Cooldown = 6;
            CurrentCooldown = 0;
            Type = AbilityType.Normal;

            OverridesGetBasicAttackCells = true;
        }

        public override void Awake()
        {
           ParentCharacter.BeforeBasicAttack += (Character character, ref int value) =>
            {
                if (character.ParentCell.Effects.ContainsType(typeof(HexCellEffects.Conflagration)))
                    value = (int) (value * (AdditionalDamagePercent / 100f + 1));
            };   
        }

        public override List<HexCell> GetBasicAttackCells()
        {
            List<HexCell> cellRange;
            switch (ParentCharacter.Type)
			{
				case FightType.Ranged:
					cellRange = ParentCharacter.ParentCell.GetNeighbors(ParentCharacter.BasicAttackRange.Value, false, false, true);
					break;
				case FightType.Melee:
					cellRange = ParentCharacter.ParentCell.GetNeighbors(ParentCharacter.BasicAttackRange.Value, true, false, true);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
            IEnumerable<HexCell> cellsWithConflagrationAndEnemyCharacters = HexMapDrawer.Instance.Cells
                .Where(c => c.Effects.Any(e => e.GetType() == typeof(HexCellEffects.Conflagration)))
                .Where(c => c.CharacterOnCell != null && c.CharacterOnCell.Owner != ParentCharacter.Owner);
            cellRange.AddRange(cellsWithConflagrationAndEnemyCharacters);
            return cellRange.Distinct().ToList();
        }

        public override string GetDescription()
        {
            return string.Format(
                @"{0} wywołuje Pożar na wskazanym obszarze o promieniu 6.
{0} może atakować wrogów znajdujących się na terenie Pożaru podstawowymi atakami, zadając 50% dodatkowych obrażeń, niezależnie od tego gdzie sama się znajduje.
Zasięg: {1}	Czas trwania: {2} Czas odnowienia: {3}",
                ParentCharacter.Name, Radius, EffectTime, Cooldown);
        }

        public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range);

        protected override void Use()
        {
            List<HexCell> cellRange = GetRangeCells();
            Active.Prepare(this, cellRange, false, false);
            Active.AirSelection.Enable(AirSelection.SelectionShape.Circle, Radius);
        }

        public override void Use(List<HexCell> cells)
        {
            try
            {
                cells.ForEach(c => c.Effects.Add(new HexCellEffects.Conflagration(EffectTime, c, ParentCharacter)));
                OnUseFinish();
            }
            catch (Exception e)
            {
                MessageLogger.DebugLog(e.Message);
                OnFailedUseFinish();
            }
        }
    }
}