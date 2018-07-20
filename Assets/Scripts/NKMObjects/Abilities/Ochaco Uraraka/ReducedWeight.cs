using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Ochaco_Uraraka
{
    public class ReducedWeight : Ability, IClickable, IUseable
    {
        public ReducedWeight() : base(AbilityType.Normal, "Reduced Weight", 3)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override List<HexCell> GetRangeCells() => ParentCharacter.GetBasicAttackCells();
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyFriendsOf(Owner);

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} nakłada na sojusznika efekt Zero Gravity,
dodatkowo podwaja mu szybkość na jedną fazę.

Zasięg: jak ataku podstawowego
Czas odnowienia: {Cooldown}";

        public void Click() => Active.Prepare(this, GetTargetsInRange());

        public void Use(List<HexCell> cells) => Use(cells[0].CharacterOnCell);

        private void Use(Character character)
        {
            ZeroGravity.AddEffect(character);
            character.Effects.Add(new StatModifier(1, character.Speed.RealValue, character, StatType.Speed, Name));
            Finish();
        }
    }
}