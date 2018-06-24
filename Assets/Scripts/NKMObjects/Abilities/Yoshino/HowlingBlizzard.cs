using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Yoshino
{
    public class HowlingBlizzard : Ability, IClickable, IEnableable
    {
        private const int MaxDuration = 4;
        private const int Range = 5;
        private const int SpeedDecrease = 3;
        private const int DamagePerPhase = 10;

        private int _currentDuration;
        
        public HowlingBlizzard() : base(AbilityType.Ultimatum, "Howling Blizzard", 6)
        {
            OnAwake += () =>
            {
                Validator.ToCheck.Remove(Validator.IsNotOnCooldown);
                Validator.ToCheck.Add(()=>Validator.IsNotOnCooldown()||IsEnabled);
                Active.Phase.PhaseFinished += () =>
                {
                    if (!IsEnabled) return;
                    _currentDuration++;
                    if (_currentDuration > 4) Disable();
                    else
                    {
                        //refresh the effect
                        RemoveHexEffects();
                        AddHexEffectsInRange();
                    }
                };
                ParentCharacter.AfterMove += () =>
                {
                    if (!IsEnabled) return;
                    //refresh the effect
                    RemoveHexEffects();
                    AddHexEffectsInRange();
                };
            };
            
        }

        public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);

        public override string GetDescription()
        {
            string desc = 
$@"{ParentCharacter.Name} otacza się Zamiecią na max. {MaxDuration} tury.
Wrogowie znajdujący się wewnątrz zamieci zostają spowolnieni do {SpeedDecrease}.
Po zakończeniu efektu, {ParentCharacter.Name} zada obrażenia magiczne wszystkim wrogom w obszarze,
tym większe im dłużej umiejętność była aktywna - {DamagePerPhase} /fazę.
Promień: {Range}    Czas odnowienia: {Cooldown}";
            if (IsEnabled) desc += $@"
Umiejętność jest włączona od {_currentDuration} faz.";
            return desc;
        }

        public void Click()
        {
            if(!IsEnabled) Enable();
            else Disable();
        }

        private void Enable()
        {
            Active.MakeAction();
            IsEnabled = true;
            _currentDuration = 1;
            AddHexEffectsInRange();
            OnUseFinish();
        }

        private void AddHexEffectsInRange() => GetRangeCells().ForEach(c =>
            c.Effects.Add(new HexCellEffects.HowlingBlizzard(1, c, ParentCharacter, SpeedDecrease, Name)));

        private void RemoveHexEffects() =>
            HexMapDrawer.Instance.Cells.ForEach(c => c.Effects.RemoveAll(e => e.Name == Name));

        private void Disable()
        {
            RemoveHexEffects();
            IsEnabled = false;
            var targets = GetTargetsInRange();
            var chara = targets.GetCharacters();
            foreach (var c in chara)
            {
                var damage = new Damage(_currentDuration*DamagePerPhase, DamageType.Magical);
                ParentCharacter.Attack(c, damage);
            }
//            GetTargetsInRange().GetCharacters().ForEach(c =>
//            {
//                var damage = new Damage(_currentDuration*DamagePerPhase, DamageType.Magical);
//                ParentCharacter.Attack(c, damage);
//            });
            _currentDuration = 0;
        }

        public bool IsEnabled { get; private set; }
    }
}