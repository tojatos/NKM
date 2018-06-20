using System.Collections.Generic;
using System.Linq;
using Extensions;
using JetBrains.Annotations;
using Managers;
using UI.CharacterUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using NKMObject = NKMObjects.Templates.NKMObject;

namespace UI
{
	public class UIManager : SingletonMonoBehaviour<UIManager>
	{
		private static Game Game => GameStarter.Instance.Game;
		private static Active Active => Game.Active;

		private SpriteSelect _spriteSelect;

		public GameObject CancelButton;
		public GameObject AbilityButtons;
		public GameObject CharacterUI;

		public GameObject EndTurnImage;
		public GameObject HourglassImage;

		public Text ActivePlayerText;
		public Text ActivePhaseText;
		public Text ActiveCharacterText;

		public bool ForcePlacingChampions { private get; set; }
		private static bool CanClickEndTurnButton =>
			!(Game.Active.Phase.Number == 0 || Game.Active.Turn.CharacterThatTookActionInTurn == null &&
			  Game.Active.GamePlayer.Characters.Any(c => c.CanTakeAction && c.IsOnMap));

		public void Init()
		{
			Stats.Instance.Init();
			Tooltip.Instance.Init();
			_spriteSelect = SpriteSelect.Instance;
			EndTurnImage.AddTrigger(EventTriggerType.PointerClick, e => EndTurnImageClick());
			CancelButton.AddTrigger(EventTriggerType.PointerClick, e => Game.Active.Cancel());
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
			List<NKMObject> characters = new List<NKMObject>(Game.Active.GamePlayer.Characters.Where(c => !c.IsOnMap && c.IsAlive));
			_spriteSelect.Open(characters, FinishUseCharacter, "Wystaw postać", "Zakończ wybieranie postaci");
		}
		private void FinishUseCharacter()
		{
			if (_spriteSelect.SelectedObjects.Count != 1) return;

			Game.HexMapDrawer.RemoveHighlights();
			Game.Active.GamePlayer.GetSpawnPoints().Where(sp => sp.CharacterOnCell == null).ToList().ForEach(c => c.AddHighlight(Highlights.RedTransparent));
			Game.Active.NkmObject = Game.Active.GamePlayer.Characters.Single(c => c.Name == _spriteSelect.SelectedObjects[0].Name);
			_spriteSelect.Close();
		}

		private void Update()
		{
			if (Game==null) return;

			if (Game.Active.Phase.Number == 0)
			{
				if (ForcePlacingChampions && !_spriteSelect.IsOpened && Game.Active.NkmObject == null && Game.Active.GamePlayer.HasFinishedSelecting)
				{
					List<NKMObject> characters = new List<NKMObject>(Game.Active.GamePlayer.Characters.Where(c => !c.IsOnMap && c.IsAlive));
					_spriteSelect.Open(characters, FinishUseCharacter, "Wystaw postać", "Zakończ wybieranie postaci");
			}
			}
			Tooltip.Instance.gameObject.ToggleIf(!Tooltip.Instance.IsActive);
			CharacterUI.ToggleIf(Game.Active.CharacterOnMap == null);
			if (Game.Active.CharacterOnMap != null) ActiveCharacterText.text = Game.Active.CharacterOnMap.Name;
			EndTurnImage.ToggleIf(!CanClickEndTurnButton);
			bool isActiveUse = Game.Active.IsActiveUse;
			AbilityButtons.ToggleIf(isActiveUse);
			CancelButton.ToggleIf(!isActiveUse);
			HourglassImage.ToggleIf(isActiveUse || Active.CharacterOnMap!=null && Active.CharacterOnMap.Owner != Active.GamePlayer || Active.Turn.CharacterThatTookActionInTurn != null);
		}

		private static void EndTurnImageClick()
		{
			if (Game.Active.Phase.Number == 0) return;
			if (CanClickEndTurnButton) Game.Active.Turn.Finish();
		}
		
		private static void HourglassImageClick()
		{
            if(Active.CharacterOnMap.Owner != Active.GamePlayer) return;
            Active.TakeActionWithCharacter();
        }

	}
}
