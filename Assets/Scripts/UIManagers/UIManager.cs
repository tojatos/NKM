using System.Collections.Generic;
using System.Linq;
using Helpers;
using Hex;
using JetBrains.Annotations;
using Managers;
using MyGameObjects.MyGameObject_templates;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagers
{
	public class UIManager : SingletonMonoBehaviour<UIManager>
	{
		private Game Game;

		private SpriteSelect SpriteSelect;
		private HexMapDrawer HexMapDrawer;

		public GameObject HexMapUI;
		public GameObject MainGameUI;
		public GameObject TopPanelUI;

		public GameObject PlaceCharacterButton;
		public GameObject UseItemButton;
		public GameObject UsePotionButton;
		public GameObject CancelButton;
		public GameObject TakeActionWithCharacterButton;

		public GameObject EndTurnButton;

		public List<GameObject> GameUI { get; private set; }
		public List<GameObject> UseButtons { get; private set; }

		private List<GameObject> CancelButtons;

		public Text ActivePlayerText;
		public Text ActivePhaseText;

		public bool ForcePlacingChampions { private get; set; }

		//private void Awake() => Init();
		private List<GameObject> _ui;
		/// <summary>
		/// On set: Hide previous UI and show new. If set to null show GameUI.
		/// </summary>
		public List<GameObject> VisibleUI
		{
			get { return _ui; }
			set
			{
				_ui?.Hide();
				_ui = value ?? GameUI;
				_ui.Show();
			}
		}
		public void Init()
		{
			Game = GameStarter.Instance.Game;
			HexMapDrawer = HexMapDrawer.Instance;
			SpriteSelect = SpriteSelect.Instance;

			Game.Active.UIManager = this;
			GameUI = new List<GameObject>
			{
				MainGameUI,
				HexMapUI,
				TopPanelUI
			};
			UseButtons = new List<GameObject>
			{
				PlaceCharacterButton,
				UseItemButton,
				UsePotionButton
			};

			CancelButtons = new List<GameObject>
			{
				CancelButton
			};
			//UseResurrectionSelect = new UseResurrectionSelect(Instantiate(MultipleDropdownsPrefab, gameObject.transform));
			Game.Active.Buttons = UseButtons;
		}
		public void UpdateActivePlayerUI() => ActivePlayerText.GetComponent<Text>().text = Game.Active.GamePlayer.Name;
		public void UpdateActivePhaseText() => ActivePhaseText.GetComponent<Text>().text = Game.Active.Phase.Number.ToString();

		//TODO: Generic methods?


		[UsedImplicitly]
		public void OpenUseCharacterSelect()
		{
			var characters = new List<MyGameObject>(Game.Active.GamePlayer.Characters.Where(c => !c.IsOnMap && c.IsAlive));
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
					var characters = new List<MyGameObject>(Game.Active.GamePlayer.Characters.Where(c => !c.IsOnMap && c.IsAlive));
					SpriteSelect.Open(characters, FinishUseCharacter, "Wystaw postać", "Zakończ wybieranie postaci");
			}
			}
			UpdateButtons();
			Tooltip.Instance.gameObject.ToggleIf(!Tooltip.Instance.IsActive);
			CharacterStats.Instance.gameObject.ToggleIf(Game.Active.CharacterOnMap == null);
			CharacterFace.Instance.gameObject.ToggleIf(Game.Active.CharacterOnMap == null);
			TakeActionWithCharacterButton.ToggleIf(!(Game.Active.CharacterOnMap != null && Game.Active.Turn.CharacterThatTookActionInTurn == null && Game.Active.CharacterOnMap.Owner == Game.Active.GamePlayer && Game.Active.CharacterOnMap.TookActionInPhaseBefore == false));
			if (Input.GetMouseButtonDown(1)|| Input.GetKeyDown(KeyCode.Escape))
			{
				if (CharacterInfo.Instance.gameObject.activeSelf)
				{
					Game.UIManager.VisibleUI = GameUI;
				}
			}
		}
		private void UpdateButtons()
		{
			if (Game.Active.Phase.Number == 0)
			{
				EndTurnButton.GetComponent<Button>().ToggleIf(true);
			}
			else
			{
				EndTurnButton.GetComponent<Button>().ToggleIf(Game.Active.Turn.CharacterThatTookActionInTurn == null && Game.Active.GamePlayer.Characters.Any(c => c.CanTakeAction && c.IsOnMap));
			}
			if (Game.Active.IsActiveUse)
			{
				Game.Active.Buttons = CancelButtons;
			}
			else if (Game.Active.CharacterOnMap != null)
			{
			}
			else
			{
				Game.Active.Buttons = UseButtons;
				PlaceCharacterButton.GetComponent<Button>().ToggleIf(Game.Active.GamePlayer.Characters.All(c => c.IsOnMap || !c.IsAlive) || Game.Active.Turn.WasCharacterPlaced || Game.Active.GamePlayer.GetSpawnPoints().All(sp => sp.CharacterOnCell != null));
			}
		}

		[UsedImplicitly]
		public void EndTurnButtonClick()
		{
			Game.Active.Turn.Finish();
		}
		[UsedImplicitly]
		public void CancelButtonClick()
		{
			Game.Active.Cancel();
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
