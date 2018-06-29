using NKMObjects.Templates;

namespace NKMObjects.Abilities.Kurogane_Ikki
{
    public class RakudaiKishi : Ability
    {
        private const int HealAmount = 15;
        public RakudaiKishi() : base(AbilityType.Passive, "Rakudai Kishi")
        {
            OnAwake += () => Game.Players.FindAll(p => p != Owner).ForEach(p =>
                p.Characters.ForEach(c => c.OnDeath += () =>
                {
                    if(!ParentCharacter.IsAlive) return;
                    ParentCharacter.Heal(ParentCharacter, HealAmount);
                }));
        }

        public override string GetDescription() =>
            $"Przy każdej śmierci wrogiej postaci {ParentCharacter.Name} leczy się za {HealAmount} HP.";
    }
}