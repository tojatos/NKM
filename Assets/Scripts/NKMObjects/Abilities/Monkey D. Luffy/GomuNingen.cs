using NKMObjects.Templates;

namespace NKMObjects.Abilities.Monkey_D._Luffy
{
    public class GomuNingen : Ability, IEnableable
    {
        private int _timesAttacked; 
        public GomuNingen() : base(AbilityType.Passive, "Gomu Ningen")
        {
            OnAwake += () => ParentCharacter.BeforeBeingBasicAttacked += (character, damage) =>
            {
                if (!IsEnabled) _timesAttacked++;
                else if (character.Type == FightType.Ranged) damage.Value = 0;
            };
        }

        public override string GetDescription() => $"{ParentCharacter.Name} blokuje co drugi podstawowy atak zasięgowy.";
        public bool IsEnabled => _timesAttacked % 2 == 0;
    }
}