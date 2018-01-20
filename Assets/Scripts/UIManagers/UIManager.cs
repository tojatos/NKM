using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hex;
using JetBrains.Annotations;
using MyGameObjects.MyGameObject_templates;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagers
{
	public class UIManager : SingletonMonoBehaviour<UIManager>
	{
		private Active Active;
		private SpriteSelect SpriteSelect;
		private HexMapDrawer HexMapDrawer;

		public GameObject HexMapUI;
		public GameObject MainGameUI;

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

		private void Awake() => InitializeStartGameScene();

		private void InitializeStartGameScene()
		{
			HexMapDrawer = HexMapDrawer.Instance;
			Active = Active.Instance;
			SpriteSelect = SpriteSelect.Instance;

			Active.UIManager = this;
			GameUI = new List<GameObject>
			{
				MainGameUI,
				HexMapUI
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
			Active.Buttons = UseButtons;
		}
		public void UpdateActivePlayerUI() => ActivePlayerText.GetComponent<Text>().text = Active.Player.Name;
		public void UpdateActivePhaseText() => ActivePhaseText.GetComponent<Text>().text = Active.Phase.Number.ToString();

		//TODO: Generic methods?


		[UsedImplicitly]
		public void OpenUseCharacterSelect()
		{
			var characters = new List<MyGameObject>(Active.Player.Characters.Where(c => !c.IsOnMap && c.IsAlive));
			SpriteSelect.Open(characters, SpriteSelect.FinishUseCharacter, "Wystaw postać", "Zakończ wybieranie postaci");
		}


		private void Update()
		{
			if (Active.Phase.Number == 0)
			{
				if (ForcePlacingChampions && !SpriteSelect.gameObject.activeSelf && Active.MyGameObject == null && Active.Player.HasFinishedSelecting)
				{
					var characters = new List<MyGameObject>(Active.Player.Characters.Where(c => !c.IsOnMap && c.IsAlive));
					SpriteSelect.Open(characters, SpriteSelect.FinishUseCharacter, "Wystaw postać", "Zakończ wybieranie postaci");
			}
			}
			UpdateButtons();
			Tooltip.Instance.gameObject.ToggleIf(!Tooltip.Instance.IsActive);
			//ToggleIf(!Tooltip.Instance.IsActive, Tooltip.Instance.gameObject);
			//ToggleIf(Active.CharacterOnMap==null, CharacterStats.Instance.gameObject);
			CharacterStats.Instance.gameObject.ToggleIf(Active.CharacterOnMap == null);
			//ToggleIf(Active.Instance.CharacterOnMap == null, CharacterFace.Instance.gameObject);
			CharacterFace.Instance.gameObject.ToggleIf(Active.Instance.CharacterOnMap == null);
			//ToggleIf(!(Active.CharacterOnMap != null && Active.Turn.CharacterThatTookActionInTurn==null && Active.CharacterOnMap.Owner == Active.Player &&Active.CharacterOnMap.TookActionInPhaseBefore == false), TakeActionWithCharacterButton);
			TakeActionWithCharacterButton.ToggleIf(!(Active.CharacterOnMap != null && Active.Turn.CharacterThatTookActionInTurn == null && Active.CharacterOnMap.Owner == Active.Player && Active.CharacterOnMap.TookActionInPhaseBefore == false));
			if (Input.GetMouseButtonDown(1)|| Input.GetKeyDown(KeyCode.Escape))
			{
				if (CharacterInfo.Instance.gameObject.activeSelf)
				{
					Active.UI = GameUI;
				}
			}
		}

		private void UpdateButtons()
		{
			if (Active.Phase.Number == 0)
			{
				//ToggleIf(true, EndTurnButton.GetComponent<Button>());
				EndTurnButton.GetComponent<Button>().ToggleIf(true);
			}
			else
			{
				//ToggleIf(Active.Turn.CharacterThatTookActionInTurn == null && Active.Player.Characters.Any(c => c.CanTakeAction && c.IsOnMap), EndTurnButton.GetComponent<Button>());
				EndTurnButton.GetComponent<Button>().ToggleIf(Active.Turn.CharacterThatTookActionInTurn == null && Active.Player.Characters.Any(c => c.CanTakeAction && c.IsOnMap));
			}
			if (Active.IsActiveUse)
			{
				Active.Buttons = CancelButtons;
			}
			else if (Active.CharacterOnMap != null)
			{
			}
			else
			{
				Active.Buttons = UseButtons;
				//ToggleIf(Active.Player.Characters.All(c => c.IsOnMap || !c.IsAlive) || Active.Turn.WasCharacterPlaced || Active.Player.GetSpawnPoints().All(sp=>sp.CharacterOnCell!=null), PlaceCharacterButton.GetComponent<Button>());
				PlaceCharacterButton.GetComponent<Button>().ToggleIf(Active.Player.Characters.All(c => c.IsOnMap || !c.IsAlive) || Active.Turn.WasCharacterPlaced || Active.Player.GetSpawnPoints().All(sp => sp.CharacterOnCell != null));
				//ToggleIf(!Active.Player.Items.Any(), UseItemButton.GetComponent<Button>());
				//ToggleIf(!Active.Player.Potions.Any(), UsePotionButton.GetComponent<Button>());
			}
		}



		private IEnumerator SelectAndInitializeThings()
		{
			var allCharacters = new List<MyGameObject>(AllMyGameObjects.Instance.Characters);
			SpriteSelect.Instance.Open(allCharacters, SpriteSelect.Instance.FinishSelectingCharacters, "Wybór postaci", "Zakończ wybieranie postaci");
			yield return new WaitUntil(() => Active.Player.HasSelectedCharacters);
			//ItemSelect.Open();TODO
			//yield return new WaitUntil(() => Active.Player.HasSelectedItems);
			//PotionSelect.Open();TODO
			//yield return new WaitUntil(() => Active.Player.HasSelectedPotions);
			Active.UI = GameUI;
			HexMapDrawer.TriangulateCells(); //clicking on map does not work without triangulating here for some reason
			Active.Turn.Finish();
		}

		//needed to call a corountine outside of a monobehaviour class
		public void StartSelectAndInitializeThings()
		{
			StartCoroutine(SelectAndInitializeThings());
		}

		[UsedImplicitly]
		public void EndTurnButtonClick()
		{
			Active.Turn.Finish();
		}
		[UsedImplicitly]
		public void CancelButtonClick()
		{
			Active.Cancel();
		}
		[UsedImplicitly]
		public void TakeActionWithCharacter()
		{
			//Active.Turn.CharacterThatTookActionInTurn = Active.CharacterOnMap;
			Active.CharacterOnMap.InvokeJustBeforeFirstAction();
			Active.Turn.Finish();
		}
	}
}
