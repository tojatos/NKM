using System.Collections.Generic;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Sakai_Yuuji
{
    public class Grammatica : Ability, IClickable, IUseableCell, IUseableCharacter
    {
        private const int HealthPercentDamage = 25;
        private const int Range = 7;

        public event Delegates.CharacterCharacter BeforeGrammatica;
        public event Delegates.CharacterCharacterCell AfterGrammatica;
        
        public Grammatica(Game game) : base(game, AbilityType.Ultimatum, "Grammatica", 5)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
            OnAwake += () => Validator.ToCheck.Add(() => GetMoveCells().Count > 0);
            CanUseOnGround = false;
        }

        public override string GetDescription() =>
            $@"{ParentCharacter.Name} teleportuje się do wybranej postaci.
Jeśli jest ona przeciwnikiem, to zadaje jej {HealthPercentDamage}% max. HP jako obrażenia nieuchronne.
Następnie wraca z wybraną postacią w miejsce, na którym stał przed użyciem tej umiejętności, a wybrana postać pojawia się obok niego.
Zasięg: {Range}    Czas odnowienia: {Cooldown}";

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereCharacters();
        private List<HexCell> GetMoveCells() => GetNeighboursOfOwner(1).FindAll(c => c.IsFreeToStand);

        public void Click() => Active.Prepare(this, GetTargetsInRange());

        private Character _target;
        public void Use(HexCell targetCell)
        {
            BeforeGrammatica?.Invoke(ParentCharacter, _target); 
            ParentCharacter.TryToTakeTurn();
            if (_target.IsEnemyFor(Owner))
            {
                int dmg = (int)(_target.HealthPoints.BaseValue * HealthPercentDamage / 100f);
                ParentCharacter.Attack(this, _target, new Damage(dmg, DamageType.True));
            }
            AfterGrammatica?.Invoke(ParentCharacter, _target, targetCell); 
            _target.MoveTo(targetCell);
            Finish();
        }
        public void Use(Character character)
        {
            _target = character;
            Active.Prepare(this, GetMoveCells());
        }
    }
}
