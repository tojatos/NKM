﻿using Extensions;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Ochaco_Uraraka
{
    public class ZeroGravity : Ability
    {
        private const int FlyingDuration = 4;
        public ZeroGravity() : base(AbilityType.Passive, "Zero Gravity")
        {
            OnAwake += () =>
            {
                ParentCharacter.CanAttackAllies = true;
                ParentCharacter.BasicAttack = character =>
                {
                    if (character.IsEnemyFor(Owner)) ParentCharacter.DefaultBasicAttack(character);
                    AddEffect(character);

                    ParentCharacter.HasUsedBasicAttackInPhaseBefore = true;
                };

            };
        }

        public static void AddEffect(NKMCharacter character) => character.Effects.Add(new Flying(FlyingDuration, character, "Zero Gravity"));
        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} może atakować sojuszników, ale nie wyrządza im szkód.
Postacie zaatakowane przez nią potrafią latać przez {FlyingDuration} fazy.";
    }
}