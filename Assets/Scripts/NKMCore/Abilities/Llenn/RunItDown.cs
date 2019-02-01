﻿using NKMCore.Templates;

namespace NKMCore.Abilities.Llenn
{
    public class RunItDown : Ability, IClickable, IEnableable
    {
        private const int TimesToRun = 3;
        private int _timesRun;
        public RunItDown(Game game) : base(game, AbilityType.Ultimatum, "Run It Down", 6)
        {
            OnAwake += () =>
            {
                ParentCharacter.AfterBasicMove += cellList =>
                {
                    if (!IsEnabled) return;
                    _timesRun++;
                    ParentCharacter.HasFreeAttackUntilEndOfTheTurn = true;
                    if (_timesRun >= TimesToRun) return;
                    ParentCharacter.HasFreeMoveUntilEndOfTheTurn = true;
                };
                Active.Turn.TurnFinished += character =>
                {
                    if (!IsEnabled) return;
                    if (_timesRun >= TimesToRun) Disable();
                };
            };

        }

        public override string GetDescription()
        {
            string desc = $@"{ParentCharacter.Name} może się poruszyć {TimesToRun} razy w tej turze.
Po każdym ruchu może użyć podstawowego ataku.
Czas odnowienia: {Cooldown}";
            if (IsEnabled) desc += $"\nLiczba pozostałych przebiegnięć: {TimesToRun - _timesRun}";
            return desc;
        }

        public void Click()
        {
			ParentCharacter.TryToTakeTurn();
            IsEnabled = true;
            ParentCharacter.HasFreeMoveUntilEndOfTheTurn = true;
            Finish();
        }

        private void Disable()
        {
            IsEnabled = false;
            _timesRun = 0;
        }

        public bool IsEnabled { get; private set; }
    }
}