using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NKMCore;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;
using Unity.Extensions;
using Unity.Hex;
using Unity.Managers;
using Unity.UI.CharacterUI;
using Unity.UI.HexCellUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Effects = Unity.UI.CharacterUI.Effects;

namespace Unity.UI
{
	public class UIManager : SingletonMonoBehaviour<UIManager>
	{
		private static Game Game => GameStarter.Instance.Game;
		private static Active Active => Game.Active;
		private static HexMap HexMap => Game.HexMap;
		private static ConsoleDrawer ConsoleDrawer => ConsoleDrawer.Instance;

		private SpriteSelect _spriteSelect;

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

		public bool ForcePlacingChampions { private get; set; }
		public static bool CanClickEndTurnButton =>
			!(Game.Active.Phase.Number == 0 || Game.Active.Turn.CharacterThatTookActionInTurn == null &&
			  Game.Active.GamePlayer.Characters.Any(c => (c.CanWait || c.CanTakeAction) && c.IsOnMap) || Active.AbilityToUse != null);

		public void Init() //TODO
		{
			Stats.Instance.Init();
			Tooltip.Instance.Init();
			_spriteSelect = SpriteSelect.Instance;
			EndTurnImage.AddTrigger(EventTriggerType.PointerClick, e => EndTurnImageClick());
			CancelButton.AddTrigger(EventTriggerType.PointerClick, e => Game.Action.Cancel());
			HourglassImage.AddTrigger(EventTriggerType.PointerClick, e => HourglassImageClick());
			ActivePlayerText.gameObject.AddSetTooltipEvent("Nazwa aktywnego gracza");
			ActivePlayerText.gameObject.AddRemoveTooltipEvent();
			ActivePhaseText.gameObject.AddSetTooltipEvent("Numer fazy");
			ActivePhaseText.gameObject.AddRemoveTooltipEvent();

		}
		public void UpdateActivePlayerUI() => ActivePlayerText.SetText(Game.Active.GamePlayer.Name);
		public void UpdateActivePhaseText() => ActivePhaseText.SetText(Game.Active.Phase.Number.ToString());

		[UsedImplicitly]
		public void OpenUseCharacterSelect()
		{
			List<Character> characters = new List<Character>(Game.Active.GamePlayer.Characters.Where(c => !c.IsOnMap && c.IsAlive));
			_spriteSelect.Open(characters, FinishUseCharacter, "Wystaw postać", "Zakończ wybieranie postaci");
		}
		private void FinishUseCharacter()
		{
			if (_spriteSelect.SelectedObjects.Count != 1) return;

			HexMapDrawer.Instance.RemoveHighlights();
			Active.GamePlayer.GetSpawnPoints(HexMap).FindAll(c => c.IsFreeToStand).Select(Active.SelectDrawnCell).ToList().ForEach(c => c.AddHighlight(Highlights.RedTransparent));
			Active.SelectedCharacterToPlace = Active.GamePlayer.Characters.Single(c => c.Name == _spriteSelect.SelectedObjects[0].Name);
			_spriteSelect.Close();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.F2))
				ConsoleDrawer.Toggle();

			if (Game==null) return;

			if (Game.Active.Phase.Number == 0)
			{
				if (ForcePlacingChampions && !_spriteSelect.IsOpened && Active.SelectedCharacterToPlace == null && Active.GamePlayer.HasFinishedSelecting)
				{
					List<Character> characters = new List<Character>(Active.GamePlayer.Characters.Where(c => !c.IsOnMap && c.IsAlive));
					_spriteSelect.Open(characters, FinishUseCharacter, "Wystaw postać", "Zakończ wybieranie postaci");
			}
			}
			Tooltip.Instance.gameObject.ToggleIf(!Tooltip.Instance.IsActive);
			CharacterUI.ToggleIf(Game.Active.Character == null);
			if (Active.Character != null) ActiveCharacterText.text = Active.Character.Name;
			if (Active.SelectedCell != null) ActiveHexCellText.text = Active.SelectedCell.Type.ToString();
			EndTurnImage.ToggleIf(!CanClickEndTurnButton);
			bool isActiveUse = Game.Active.IsActiveUse;
			AbilityButtons.ToggleIf(isActiveUse);
			CancelButton.ToggleIf(!isActiveUse);
			HourglassImage.ToggleIf(isActiveUse || Active.Character!=null && !Active.Character.CanWait);
			
			HexCellUI.ToggleIf(Active.SelectedCell == null);
            if(Active.SelectedCell!=null) HexImage.Instance.UpdateImage();
		}

		private static void EndTurnImageClick()
		{
			if (Game.Active.Phase.Number == 0) return;
			if (CanClickEndTurnButton) Game.Action.FinishTurn();//Game.Active.Turn.Finish();
		}
		
		private static void HourglassImageClick()
		{
            if(Active.Character.Owner != Active.GamePlayer) return;
			Game.Action.TakeTurn(Active.Character);
			Game.Action.FinishTurn();
        }

		public void AddUITriggers(Character character)
		{
            character.HealthPoints.StatChanged += () =>
            {
                if (Active.Character == character) MainHPBar.Instance.UpdateHPAmount(character);
            };
            Active.AfterSelect += chara =>
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
