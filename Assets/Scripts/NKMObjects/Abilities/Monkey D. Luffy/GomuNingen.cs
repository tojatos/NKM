using NKMObjects.Templates;

namespace NKMObjects.Abilities.Monkey_D._Luffy
{
    public class GomuNingen : Ability
    {
        public GomuNingen() : base(AbilityType.Passive, "Gomu Ningen")
        {
            OnAwake += () => ParentCharacter.BeforeBeingBasicAttacked += (character, damage) =>
            {
                if (character.Type == FightType.Ranged) damage.Value = 0;
            };
        }

        public override string GetDescription() => $"{ParentCharacter.Name} jest odporny na podstawowe ataki zasięgowe.";
    }
}