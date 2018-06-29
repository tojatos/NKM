using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Kirito
{
    public class StarburstStream : Ability, IClickable, IEnableable, IUseable
    {
        private const int Range = 3;
        private const int AttackTimes = 16;
        private const int Damage = 2;
        public StarburstStream() : base(AbilityType.Ultimatum, "Starburst Stream", 6)
        {
            OnAwake += () =>
            {
                Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
                ParentCharacter.AfterBasicAttack += (character, damage) =>
                {
                    if(!IsEnabled) return;
                    if (_gotFreeAttackThisTurn) return;
                    ParentCharacter.HasFreeAttack = true;
                    _gotFreeAttackThisTurn = true;
                };
                Active.Turn.TurnStarted += player => _gotFreeAttackThisTurn = false;
            };
        }

        public override List<HexCell> GetRangeCells() =>
            ParentCharacter.ParentCell.GetNeighbors(Range, SearchFlags.StraightLine);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);

        public override string GetDescription() =>
$@"{ParentCharacter.Name} atakuje przeciwnika {AttackTimes} razy.
Każdy cios zadaje {Damage} pkt obrażeń nieuchronnych.
Po użyciu tej umiejętnoości Kirito może atakować 2 razy na turę.
Efekt jest trwały.
Zasięg: {Range}    Czas odnowienia: {Cooldown}";

        private bool _gotFreeAttackThisTurn;

        public void Click() => Active.Prepare(this, GetTargetsInRange());
	    public void Use(List<HexCell> cells) => Use(cells[0].CharacterOnCell);

        private void Use(Character character)
        {
            Active.MakeAction();
            for (int i = 0; i < AttackTimes; i++) 
                ParentCharacter.Attack(this, character, new Damage(Damage, DamageType.True));
            if (!IsEnabled) IsEnabled = true;
            Finish();
        }

        public bool IsEnabled { get; private set; }
    }
}