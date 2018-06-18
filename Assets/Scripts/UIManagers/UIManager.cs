using System.Collections.Generic;
using System.Linq;
using Helpers;
using JetBrains.Annotations;
using Managers;
using MyGameObjects.MyGameObject_templates;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIManagers
{
	public class UIManager : SingletonMonoBehaviour<UIManager>
	{
		private Game Game;

		private SpriteSelect SpriteSelect;

//		public GameObject HexMapUI;
//		public GameObject MainGameUI;
//		public GameObject TopPanelUI;

//		public GameObject PlaceCharacterButton;
//		public GameObject UseItemButton;
//		public GameObject UsePotionButton;
		public GameObject CancelButton;
		public GameObject TakeActionWithCharacterButton;
		public GameObject CharacterUI;

		public GameObject EndTurnImage;

		private bool CanClickEndTurnButton => !(Game.Active.Phase.Number == 0 || Game.Active.Turn.CharacterThatTookActionInTurn == null && Game.Active.GamePlayer.Characters.Any(c => c.CanTakeAction && c.IsOnMap));
//		public List<GameObject> GameUI { get; private set; }
//		public List<GameObject> UseButtons { get; private set; }

//		private List<GameObject> CancelButtons;

		public Text ActivePlayerText;
		public Text ActivePhaseText;

		public bool ForcePlacingChampions { private get; set; }

		public void Init()
		{
			Game = GameStarter.Instance.Game;
			SpriteSelect = SpriteSelect.Instance;
			EndTurnImage.AddTrigger(EventTriggerType.PointerClick, e => EndTurnImageClick());
			
		}
		public void UpdateActivePlayerUI() => ActivePlayerText.SetText(Game.Active.GamePlayer.Name);
		public void UpdateActivePhaseText() => ActivePhaseText.SetText(Game.Active.Phase.Number.ToString());

		//TODO: Generic methods?


		[UsedImplicitly]
		public void OpenUseCharacterSelect()
		{
			List<MyGameObject> characters = new List<MyGameObject>(Game.Active.GamePlayer.Characters.Where(c => !c.IsOnMap && c.IsAlive));
			SpriteSelect.Open(characters, FinishUseCharacter, "Wystaw postać", "Zakończ wybieranie postaci");
		}
		private void FinishUseCharacter()
		{
			if (SpriteSelect.SelectedObjects.Count != 1) return;

			Game.HexMapDrawer.RemoveAllHighlights();
			Game.Active.GamePlayer.GetSpawnPoints().Where(sp => sp.CharacterOnCell == null).ToList().ForEach(c => c.ToggleHighlight(HiglightColor.Red));
			Game.Active.MyGameObject = Game.Active.GamePlayer.Characters.Single(c => c.Name == SpriteSelect.SelectedObjects[0].Name);
			SpriteSelect.Close();
		}

		private void Update()
		{
			if (Game==null) return;

			if (Game.Active.Phase.Number == 0)
			{
				if (ForcePlacingChampions && !SpriteSelect.IsOpened && Game.Active.MyGameObject == null && Game.Active.GamePlayer.HasFinishedSelecting)
				{
					List<MyGameObject> characters = new List<MyGameObject>(Game.Active.GamePlayer.Characters.Where(c => !c.IsOnMap && c.IsAlive));
					SpriteSelect.Open(characters, FinishUseCharacter, "Wystaw postać", "Zakończ wybieranie postaci");
			}
			}
			Tooltip.Instance.gameObject.ToggleIf(!Tooltip.Instance.IsActive);
			CharacterUI.ToggleIf(Game.Active.CharacterOnMap == null);
			EndTurnImage.ToggleIf(!CanClickEndTurnButton);
			TakeActionWithCharacterButton.ToggleIf(!(Game.Active.CharacterOnMap != null && Game.Active.Turn.CharacterThatTookActionInTurn == null && Game.Active.CharacterOnMap.Owner == Game.Active.GamePlayer && Game.Active.CharacterOnMap.TookActionInPhaseBefore == false));
		}
// TODO
//			if (Game.Active.IsActiveUse)
//			{
//				Game.Active.Buttons = CancelButtons;
//			}

		private void EndTurnImageClick()
		{
			if (Game.Active.Phase.Number == 0) return;
			if (CanClickEndTurnButton) Game.Active.Turn.Finish();
		}

		[UsedImplicitly]
		public void TakeActionWithCharacter()
		{
			//Game.Active.Turn.CharacterThatTookActionInTurn = Game.Active.CharacterOnMap;
			Game.Active.CharacterOnMap.InvokeJustBeforeFirstAction();
			Game.Active.Turn.Finish();
		}
	}
}
