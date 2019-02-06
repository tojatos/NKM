﻿using NKMCore.Templates;

namespace NKMCore.Abilities.Llenn
{
    public class PChan : Ability
    {
        private const int SpeedIncrease = 2;
        public PChan(Game game) : base(game, AbilityType.Passive, "P-Chan")
        {
            OnAwake += () =>
                Owner.Owner.Characters.ForEach(c => c.OnDeath += () => ParentCharacter.Speed.Value = ParentCharacter.Speed.RealValue + SpeedIncrease);
        }

        public override string GetDescription() =>
$@"Każda śmierć przyjaznej postaci zwiększa wolę do działania {ParentCharacter.Name},
co daje jej {SpeedIncrease} szybkości na stałe.";
    }
}