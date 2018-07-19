using Extensions;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Derieri
{
    public class ComboStar : Ability
    {
        private const int DamagePerCombo = 4;
        
        public int Combo;
        public Character ComboCharacter { get; private set; }
        private bool _wasCharacterAttackedThisPhase;
        public ComboStar() : base(AbilityType.Passive, "Combo star")
        {
            OnAwake += () =>
            {
                ParentCharacter.BeforeBasicAttack += (character, damage) =>
                {
                    if (character != ComboCharacter)
                    {
                        ComboCharacter = character;
                        Combo = 0;
                    }

                    damage.Value += Combo * DamagePerCombo;
                };
                ParentCharacter.AfterBasicAttack += (character, damage) =>
                {
                    Combo++;
                    _wasCharacterAttackedThisPhase = true;
                };
                Active.Phase.PhaseFinished += () =>
                {
                    if (!_wasCharacterAttackedThisPhase) Combo = 0;
                    _wasCharacterAttackedThisPhase = false;
                };
            };
        }

        public override string GetDescription() =>
$@"Każdy kolejny podstawowy atak {ParentCharacter.FirstName()} wykonany w jednego konkretnego wroga bez przerwy podwyższa Combo.
Każdy ładunek Combo zwiększa kolejny atak w tego wroga o {DamagePerCombo}.
Zaatakowanie innego wroga lub nie zaatakowanie w fazie resetuje Combo.

<i>Aktualne Combo: {Combo}</i>";
    }
}