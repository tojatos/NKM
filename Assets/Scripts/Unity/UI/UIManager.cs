using System.Linq;
using NKMCore;
using NKMCore.Templates;
using Unity.Extensions;
using Unity.Hex;
using Unity.UI.CharacterUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Effects = Unity.UI.CharacterUI.Effects;

namespace Unity.UI
{
    public class UIManager : SingletonMonoBehaviour<UIManager>
    {
        private Game _game;
        private Active Active => _game.Active;
        public UnityActive UnityActive;
        private static ConsoleDrawer ConsoleDrawer => ConsoleDrawer.Instance;

        public GameObject CancelButton;
        public GameObject AbilityButtons;
        public GameObject CharacterUI;
        public GameObject HexCellUI;

        public GameObject EndTurnImage;
        public GameObject HourglassImage;


        public Text ActivePlayerText;
        public Text ActivePhaseText;
        public Text ActiveCharacterText;
        public Text ActiveHexCellText;

        private bool CanClickEndTurnButton =>
            !(_game == null || _game.Active.Phase.Number == 0 || _game.Active.Turn.CharacterThatTookActionInTurn == null &&
              _game.Active.GamePlayer.Characters.Any(c => (Active.CanWait(c) || Active.CanTakeAction(c)) && c.IsOnMap) || Active.AbilityToUse != null);


        public void Init(Game game) //TODO
        {
            _game = game;
            UnityActive = new UnityActive();
            Stats.Instance.Init(game);
            Tooltip.Instance.Init();
            EndTurnImage.AddTrigger(EventTriggerType.PointerClick, e => EndTurnImageClick());
            CancelButton.AddTrigger(EventTriggerType.PointerClick, e => _game.Action.Cancel());
            HourglassImage.AddTrigger(EventTriggerType.PointerClick, e => HourglassImageClick());
            ActivePlayerText.gameObject.AddSetTooltipEvent("Nazwa aktywnego gracza");
            ActivePlayerText.gameObject.AddRemoveTooltipEvent();
            ActivePhaseText.gameObject.AddSetTooltipEvent("Numer fazy");
            ActivePhaseText.gameObject.AddRemoveTooltipEvent();

            game.Active.AfterDeselect += () => UnityActive.SelectedCell = null;
            game.Active.Turn.TurnFinished += character => UnityActive.SelectedCell = null;
            HexMapDrawer.Instance.AfterCellSelect += cell => UnityActive.SelectedCell = cell;


            game.Active.Phase.PhaseChanged += UpdateActivePhaseText;
            game.Active.Turn.TurnStarted += UpdateActivePlayerUI;
        }

        public void UpdateActivePlayerUI(GamePlayer player) => ActivePlayerText.SetText(player.Name);
        public void UpdateActivePhaseText() => ActivePhaseText.SetText(_game.Active.Phase.Number.ToString());

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
                ConsoleDrawer.Toggle();

            EndTurnImage.ToggleIf(!CanClickEndTurnButton);
            Tooltip.Instance.gameObject.ToggleIf(!Tooltip.Instance.IsActive);
            CharacterUI.ToggleIf(_game?.Active.Character == null);
            HexCellUI.ToggleIf(UnityActive?.SelectedCell == null);

            if (_game==null) return;

            if (Active.Character != null) ActiveCharacterText.text = Active.Character.Name;
            if (UnityActive.SelectedCell != null) ActiveHexCellText.text = UnityActive.SelectedCell.Type.ToString();
            bool isActiveUse = _game.Active.IsActiveUse;
            AbilityButtons.ToggleIf(isActiveUse);
            CancelButton.ToggleIf(!isActiveUse);
            HourglassImage.ToggleIf(isActiveUse || Active.Character!=null && !Active.CanWait(Active.Character));

        }

        private void EndTurnImageClick()
        {
            if (_game.Active.Phase.Number == 0) return;
            if (CanClickEndTurnButton) _game.Action.FinishTurn(); //Game.Active.Turn.Finish();
        }

        private void HourglassImageClick()
        {
            if(Active.Character.Owner != Active.GamePlayer) return;
            _game.Action.TakeTurn(Active.Character);
            _game.Action.FinishTurn();
        }

        public void AddUITriggers(Character character)
        {
            character.HealthPoints.StatChanged += (o, n) =>
            {
                if (Active.Character == character) MainHPBar.Instance.UpdateHPAmount(character);
            };
            Active.AfterCharacterSelect += chara =>
            {
                Stats.Instance.UpdateCharacterStats(chara);
                MainHPBar.Instance.UpdateHPAmount(chara);
                UI.CharacterUI.Abilities.Instance.UpdateButtons();
                Effects.Instance.UpdateButtons();
            };
            Active.AfterDeselect += () =>
            {
                HexMapDrawer.Instance.RemoveHighlights();
                Stats.Instance.UpdateCharacterStats(null);
            };

        }

    }
}
