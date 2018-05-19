using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpers;
using Hex;
using Managers;
using MyGameObjects.MyGameObject_templates;
using UIManagers;
using UnityEngine;

public class Game
{
	private GameOptions _options;

	public List<GamePlayer> Players;
	public Active Active;
	public UIManager UIManager;
	public Spawner Spawner;
	public HexMapDrawer HexMapDrawer;

	public bool IsInitialized;
	public Game()
	{
		Active = new Active(this);
	}

	public void Init(GameOptions gameOptions)
	{
		_options = gameOptions;

		Players = new List<GamePlayer>(gameOptions.Players);
		UIManager = _options.UIManager;
		HexMapDrawer = HexMapDrawer.Instance;
		Spawner = Spawner.Instance;
//		Spawner.Init(this);

//		Players.ForEach(p => Debug.Log(p.Characters.Count));
//		Debug.Log("Game started!");
		IsInitialized = true;
	}

	/// <summary>
	/// Returns true if game is successfully started,
	/// otherwise returns false.
	/// </summary>
	/// <returns></returns>
	public bool StartGame()
	{
		if (!IsInitialized) return false;

		HexMapDrawer.CreateMap(_options.Map);
		UIManager.Init();
		UIManager.VisibleUI = UIManager.GameUI;
		Active.Buttons = UIManager.UseButtons;
		MainCameraController.Instance.Init();
		CharacterAbilities.Instance.Init();
		UIManager.UpdateActivePhaseText();
		if (GameStarter.Instance.IsTesting) PlaceAllCharactersOnSpawns();
		TakeTurns();
		return true;
	}

	/// <summary>
	/// Infinite loop that manages Turns and Phases
	/// </summary>
	private async void TakeTurns()
	{
		while (true)
		{
			foreach (var player in Players) await TakeTurn(player);

			if (!IsEveryCharacterPlacedInTheFirstPhase) continue;

			if (NoCharacterOnMapCanTakeAction) Active.Phase.Finish();
		}
	}

	private bool NoCharacterOnMapCanTakeAction => Players.All(p => p.Characters.Where(c => c.IsOnMap).All(c => !c.CanTakeAction));
	private bool IsEveryCharacterPlacedInTheFirstPhase => !(Active.Phase.Number == 0 && Players.Any(p => p.Characters.Any(c => !c.IsOnMap)));

	/// <summary>
	/// Start a turn and wait for player to end it
	/// </summary>
	private async Task TakeTurn(GamePlayer player)
	{
		Active.Turn.Start(player);
		Func<bool> isTurnDone = () => Active.Turn.IsDone;
		await isTurnDone.WaitToBeTrue();
	}

	public void TryTouchingCell(HexCell touchedCell)
	{
		TouchCell(touchedCell);
	}

	public void TouchCell(HexCell touchedCell)
	{
		Players.ForEach(p=>p.Characters.ForEach(c=>Debug.Log(c.ParentCell.Coordinates)));
		
		if (Active.MyGameObject != null)
		{
			UseMyGameObject(touchedCell);

		}
		else if (Active.HexCells?.Contains(touchedCell) == true)
		{
			Active.MakeAction(touchedCell);
		}
		else
		{
			if (Active.Ability != null)
			{
				return;
			}
			//possibility of highlighting with control pressed
			if (!Input.GetKey(KeyCode.LeftControl))
			{
				HexMapDrawer.RemoveAllHighlights();
			}
			if (touchedCell.CharacterOnCell != null)
			{
				touchedCell.CharacterOnCell.Select();
//				Debug.Log(touchedCell.CharacterOnCell.Guid);
			}
			else
			{
				Active.CharacterOnMap?.Deselect();
				touchedCell.ToggleHighlight();
			}
		}
	}

	public void UseMyGameObject(HexCell cell)
	{
//		var myGameObjectType = Active.MyGameObject.GetType().BaseType;
//		Debug.Log(myGameObjectType);

		if (Active.MyGameObject.GetType() == typeof(Character))
		{
			if (Active.Turn.WasCharacterPlaced)
			{
				throw new Exception("W tej turze już była wystawiona postać!");
			}

			var activeCharacter = Active.MyGameObject as Character;
			try
			{
				Spawner.TrySpawning(cell, activeCharacter);
			}
			catch (Exception e)
			{
				MessageLogger.Instance.DebugLog(e.Message);
				throw;
			}

			Active.Turn.WasCharacterPlaced = true;
			if (Active.Phase.Number == 0)
			{
				UIManager.ForcePlacingChampions = false;
				Active.Turn.Finish();
			}
			else
			{
				Active.MyGameObject = null;
				HexMapDrawer.RemoveAllHighlights();
				activeCharacter?.Select();
			}

		}
	}

	/// <summary>
	/// Try to place all characters from game on their spawns
	/// 
	/// Dependencies:
	/// - Players.Characters
	/// - Active.Phase
	/// </summary>
	public void PlaceAllCharactersOnSpawns()
	{
		Players.ForEach(p => p.Characters.ForEach(c => TrySpawning(p, c)));
		if(Active.Phase.Number==0) Active.Phase.Finish();
	}

	private static void TrySpawning(GamePlayer p, Character c)
	{
		var spawnPoint = p.GetSpawnPoints().FirstOrDefault(cell => cell.CharacterOnCell == null);
		if (spawnPoint == null) return;

		Spawner.Instance.TrySpawning(spawnPoint, c);
	}
}