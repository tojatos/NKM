using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Ononoki_Yotsugi
{
    public class UnlimitedRulebook : Ability, IRunable
    {
        private const int TakenDamageDecreasePercent = 25;
        private const int Radius = 3;
        public UnlimitedRulebook() : base(AbilityType.Passive, "Unlimited Rulebook")
        {
            OnAwake += () => ParentCharacter.Abilities.ForEach(a => a.AfterUseFinish += Run);
        }

        public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Radius).AddOne(ParentCharacter.ParentCell);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyFriendsOf(Owner);

        public override string GetDescription() => 
$@"Po użyciu umiejętności przez {ParentCharacter.FirstName()},
ona i wszyscy sojusznicy dookoła niej będą otrzymywać obrażenia zmniejszone o {TakenDamageDecreasePercent}% w następnej fazie.

Promień: {Radius}";

        public void Run() => GetTargetsInRange().GetCharacters()
            .ForEach(c => c.Effects.Add(new TakenDamageModifier(1, -TakenDamageDecreasePercent, c, Name)));
    }
}
