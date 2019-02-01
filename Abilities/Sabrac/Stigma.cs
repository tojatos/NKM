using System;
using System.Linq;
using NKMCore.Effects;
using NKMCore.Extensions;
using NKMCore.Templates;

namespace NKMCore.Abilities.Sabrac
{
    public class Stigma : Ability
    {
        public Stigma(Game game) : base(game, AbilityType.Passive, "Stigma")
        {
            OnAwake += () =>
            {
                Action<Character> applyEffect = character =>
                {
                    character.Effects.OfType<IncreasablePoison>().Where(e => e.Name == Name).ToList().ForEach(e =>
                    {
                        e.Damage.Value = 1;
                        e.CurrentCooldown = 2;
                    });
                    character.Effects.Add(new IncreasablePoison(Game, ParentCharacter, new Damage(1, DamageType.True), 1, 2, character, Name));
                };
                ParentCharacter.AfterBasicAttack += (character, damage) => applyEffect(character);
                ParentCharacter.AfterAbilityAttack += (ability, character, damage) => applyEffect(character);
            };
        }

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} nakłada ładunek Stigmy na cele ataków lub umiejętności,
który powoduje krwawienie zadające kolejno 1 i 2 obrażeń nieuchronnych.
Dalsze atakowanie jednego celu odnawia Stigmę i nakłada kolejny ładunek.";
    }
}
