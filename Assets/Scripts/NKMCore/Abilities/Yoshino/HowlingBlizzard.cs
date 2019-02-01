using System.Collections.Generic;
using NKMCore.Effects;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Yoshino
{
    public class HowlingBlizzard : Ability, IClickable, IEnableable
    {
        private const int MaxDuration = 4;
        private const int Range = 5;
        private const int SpeedDecrease = 3;
        private const int DamagePerPhase = 10;

        private int _currentDuration;
        
        public HowlingBlizzard(Game game) : base(game, AbilityType.Ultimatum, "Howling Blizzard", 6)
        {
            OnAwake += () =>
            {
//                Validator.ToCheck.Remove(Validator.IsActivePhaseGreaterThanThree);
                Validator.ToCheck.Remove(Validator.IsNotOnCooldown);
                Validator.ToCheck.Remove(Validator.IsCharacterNotSilenced);
                Validator.ToCheck.Add(()=>Validator.IsNotOnCooldown()||IsEnabled);
                Validator.ToCheck.Add(()=>Validator.IsCharacterNotSilenced()||IsEnabled);
                Active.Phase.PhaseFinished += () =>
                {
                    if (!IsEnabled) return;
                    AddHexEffectsInRange();
                    _currentDuration++;
                    if (_currentDuration > 4) Disable();
                };
                ParentCharacter.BeforeMove += () =>
                {
                    if (!IsEnabled) return;
                    Disable();
                };
            };
        }

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner);

        public override string GetDescription()
        {
            string desc = 
$@"{ParentCharacter.Name} otacza się Zamiecią na max. {MaxDuration} tury.
Wrogowie znajdujący się wewnątrz zamieci zostają spowolnieni do {SpeedDecrease}.
Podczas trwania tego efektu {ParentCharacter.Name} nie może podejmować żadnych akcji poza wyłączeniem zamieci.
Po zakończeniu zamieci, {ParentCharacter.Name} zada obrażenia magiczne wszystkim wrogom w obszarze,
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
			ParentCharacter.TryToTakeTurn();
            IsEnabled = true;
            _currentDuration = 1;
            AddHexEffectsInRange();
            ParentCharacter.Effects.Add(new Snare(Game, MaxDuration, ParentCharacter, Name));
            ParentCharacter.Effects.Add(new Disarm(Game, MaxDuration, ParentCharacter, Name));
            ParentCharacter.Effects.Add(new Silent(Game, MaxDuration, ParentCharacter, Name));
            Finish();
        }

        private void AddHexEffectsInRange() => GetRangeCells().ForEach(c =>
            c.Effects.Add(new HexCellEffects.HowlingBlizzard(Game, 1, c, ParentCharacter, SpeedDecrease, Name)));

        private void RemoveHexEffects() =>
//            HexMapDrawer.Instance.Cells.ForEach(c => c.Effects.RemoveAll(e => e.Name == Name));
            HexMap.Cells.ForEach(c => c.Effects.FindAll(e => e.Name == Name).ForEach(e => e.Remove()));//TODO: create a list of effects to iterate on in case of 2 or more Yoshinos playing

        private void Disable()
        {
            RemoveHexEffects();
            IsEnabled = false;
            ParentCharacter.Effects.RemoveAll(e => e.Name == Name); // Remove movement disability
            GetTargetsInRange().GetCharacters().ForEach(c =>
            {
                var damage = new Damage(_currentDuration*DamagePerPhase, DamageType.Magical);
                ParentCharacter.Attack(this, c, damage);
            });
            _currentDuration = 0;
//            ParentCharacter.Select(); // Character can move immediately, and the ability button is not shown as clickable
            Active.Select(ParentCharacter);
        }

        public bool IsEnabled { get; private set; }
    }
}