using NKMCore.Effects;
using NKMCore.Templates;

namespace NKMCore.Abilities.Ryuko_Matoi
{
    public class ScissorBlade : Ability
    {
        private const int PhysicalDefenseDecrease = 5;
        private const int Duration = 2;
        public ScissorBlade(Game game) : base(game, AbilityType.Passive, "Scissor Blade")
        {
            OnAwake += () => ParentCharacter.BeforeBasicAttack += (character, damage) =>
                character.Effects.Add(new StatModifier(Game, Duration, -PhysicalDefenseDecrease, character,StatType.PhysicalDefense, Name));
        }

        public override string GetDescription() =>
$@"Podstawowe ataki {ParentCharacter.Name} zmniejszają obronę fizyczną przeciwników o {PhysicalDefenseDecrease} na {Duration} fazy.
Efekt ten nakłada się przed atakiem i może się kumulować.

Czas odnowienia: {Cooldown}";
    }
}