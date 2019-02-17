using NKMCore.Templates;

namespace NKMCore.Abilities.Sakai_Yuuji
{
    public class ReijiMaigo : Ability
    {
        private int _toNextUse = 5;
        public ReijiMaigo(Game game) : base(game, AbilityType.Passive, "Reiji Maigo")
        {
            OnAwake += () => Active.Phase.PhaseFinished += () =>
            {
                if (_toNextUse != 0)
                {
                    --_toNextUse;
                    return;
                }
                ParentCharacter.Effects.RemoveAll(e => e.Type == EffectType.Negative);
                ParentCharacter.Heal(ParentCharacter,
                    ParentCharacter.HealthPoints.BaseValue - ParentCharacter.HealthPoints.Value);
                _toNextUse = 5;
            };
        }

        public override string GetDescription() =>
            $@"Co 5 faz {ParentCharacter.Name} przywraca sobie pełne HP oraz usuwa wszystkie negatywne efekty będące na nim.
Następne użycie na końcu fazy <color=red>{Active.Phase.Number+_toNextUse}</color>";
    }
}