using System.Collections.Generic;
using System.Linq;
using NKMCore.Effects;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Ochaco_Uraraka
{
    public class ReducedWeight : Ability, IClickable, IUseableCharacter
    {
        public ReducedWeight(Game game) : base(game, AbilityType.Normal, "Reduced Weight", 3)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override List<HexCell> GetRangeCells() => ParentCharacter.GetBasicAttackCells();
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereFriendsOf(Owner);

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} nakłada na sojusznika efekt Zero Gravity,
dodatkowo podwaja mu szybkość na jedną fazę.

Zasięg: jak ataku podstawowego
Czas odnowienia: {Cooldown}";

        public void Click() => Active.Prepare(this, GetTargetsInRange());

        public void Use(Character character)
        {
			ParentCharacter.TryToTakeTurn();
            ParentCharacter.Abilities.OfType<ZeroGravity>().First().AddEffect(character);
            character.Effects.Add(new StatModifier(Game, 1, character.Speed.RealValue, character, StatType.Speed, Name));
            Finish();
        }
    }
}