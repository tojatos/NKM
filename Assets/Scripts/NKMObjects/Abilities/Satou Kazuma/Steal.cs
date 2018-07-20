using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Satou_Kazuma
{
    public class Steal : Ability, IClickable, IUseable, IEnableable
    {
        private const int Duration = 4;
        private const int Range = 6;
        public Steal() : base(AbilityType.Ultimatum, "Steal", 6)
        {
            OnAwake += () =>
            {
                Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
                Active.Phase.PhaseFinished += () =>
                {
                    if(!IsEnabled) return;
                    _currentDuration++;
                    if (_currentDuration > 4) Disable();
                };
            };
        }

        public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);
        
        public override string GetDescription()
        {
            string desc = $@"{ParentCharacter.FirstName()} kradnie zbroję przeciwnikowi, 
zerując jego bazowe statystyki obronne i dodawając je sobie.

Zasięg: {Range}    Czas trwania: {Duration}    Czas odnowienia {Cooldown}";
            if (IsEnabled) desc += $@"
Umiejętność jest włączona od {_currentDuration} faz.";
            return desc;
        }

        public void Click() => Active.Prepare(this, GetTargetsInRange());
        public void Use(List<HexCell> cells) => Use(cells[0].CharacterOnCell);


        private Character _lastTargetCharacter;
        private int _currentDuration;
        
        private void Use(Character character)
        {
            _lastTargetCharacter = character;
            _currentDuration = 1;

            IsEnabled = true;
            ParentCharacter.PhysicalDefense.Value += _lastTargetCharacter.PhysicalDefense.BaseValue;
            _lastTargetCharacter.PhysicalDefense.Value -= _lastTargetCharacter.PhysicalDefense.BaseValue;
            ParentCharacter.MagicalDefense.Value += _lastTargetCharacter.MagicalDefense.BaseValue;
            _lastTargetCharacter.MagicalDefense.Value -= _lastTargetCharacter.MagicalDefense.BaseValue;
            Finish();
        }

        private void Disable()
        {
            IsEnabled = false;
            ParentCharacter.PhysicalDefense.Value -= _lastTargetCharacter.PhysicalDefense.BaseValue;
            _lastTargetCharacter.PhysicalDefense.Value += _lastTargetCharacter.PhysicalDefense.BaseValue;
            ParentCharacter.MagicalDefense.Value -= _lastTargetCharacter.MagicalDefense.BaseValue;
            _lastTargetCharacter.MagicalDefense.Value += _lastTargetCharacter.MagicalDefense.BaseValue;
        }
        

        public bool IsEnabled { get; private set; }
    }
}