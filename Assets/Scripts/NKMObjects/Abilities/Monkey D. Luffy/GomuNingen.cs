using NKMObjects.Templates;

namespace NKMObjects.Abilities.Monkey_D._Luffy
{
    public class GomuNingen : Ability, IEnableable
    {
        private int _timesAttacked; 
        public GomuNingen(Game game) : base(game, AbilityType.Passive, "Gomu Ningen")
        {
            OnAwake += () => ParentCharacter.BeforeBeingBasicAttacked += (character, damage) =>
            {
                if (character.Type != FightType.Ranged) return;
                if(IsEnabled) damage.Value = 0;
                _timesAttacked++;
            };
        }

        public override string GetDescription() => $"{ParentCharacter.Name} blokuje co drugi podstawowy atak zasięgowy.";
        public bool IsEnabled => _timesAttacked % 2 == 0;
    }
}