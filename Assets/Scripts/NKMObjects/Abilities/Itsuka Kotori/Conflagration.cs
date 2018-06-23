﻿using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Itsuka_Kotori
{
    public class Conflagration : Ability, IClickable
    {
        private const int Range = 13;
        private const int Radius = 6;
        private const int EffectTime = 3;
        private const int DamagePercent = 50;

        public Conflagration() : base(AbilityType.Normal, "Conflagration", 6)
        {
//            Name = "Conflagration";
//            Cooldown = 6;
//            CurrentCooldown = 0;
//            Type = AbilityType.Normal;

//            OverridesGetBasicAttackCells = true;
        }

        public override void Awake()
        {
            ParentCharacter.BeforeBasicAttack += (character, damage) =>
            {
                if (character.ParentCell.Effects.ContainsType(typeof(HexCellEffects.Conflagration)))
                    damage.Value = (int) (damage.Value * (DamagePercent / 100f));
            };
            ParentCharacter.GetBasicAttackCells = () =>
            {
                var cellRange = ParentCharacter.DefaultGetBasicAttackCells();
                IEnumerable<HexCell> cellsWithConflagrationAndEnemyCharacters = HexMapDrawer.Instance.Cells
                    .Where(c => c.Effects.Any(e => e.GetType() == typeof(HexCellEffects.Conflagration)))
                    .Where(c => c.CharacterOnCell != null && c.CharacterOnCell.Owner != ParentCharacter.Owner);
                cellRange.AddRange(cellsWithConflagrationAndEnemyCharacters);
                return cellRange.Distinct().ToList();
            };
        }

        public override string GetDescription() => string.Format(
            @"{0} wywołuje Pożar na wskazanym obszarze o promieniu 6.
{0} może atakować wrogów znajdujących się na terenie Pożaru podstawowymi atakami, zadając 50% zwykłych obrażeń, niezależnie od tego gdzie sama się znajduje.
Zasięg: {1}	Czas trwania: {2} Czas odnowienia: {3}",
            ParentCharacter.Name, Range, EffectTime, Cooldown);

        public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range);

        public void ImageClick()
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