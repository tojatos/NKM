using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Extensions;
using Hex;
using Managers;
using NKMObjects.Templates;
using UI;
using UI.CharacterUI;
using UnityEngine;

public class Game
{
	private GameOptions _options;

	public List<GamePlayer> Players;
	public readonly Active Active;
	public UIManager UIManager;
	private Spawner _spawner;
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
		_spawner = Spawner.Instance;
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
//		UIManager.VisibleUI = UIManager.GameUI;
//		Active.Buttons = UIManager.UseButtons;
		MainCameraController.Instance.Init();
		Abilities.Instance.Init();
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
			foreach (GamePlayer player in Players) await TakeTurn(player);

			if (!IsEveryCharacterPlacedInTheFirstPhase) continue;

			if (NoCharacterOnMapCanTakeAction) Active.Phase.Finish();
		}
		// ReSharper disable once FunctionNeverReturns
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

	public void TouchCell(HexCell touchedCell)
	{
		if (Active.NkmObject != null)
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
				HexMapDrawer.RemoveHighlights();
			}
			if (touchedCell.CharacterOnCell != null)
			{
				touchedCell.CharacterOnCell.Select();
			}
			else
			{
				Active.CharacterOnMap?.Deselect();
				touchedCell.AddHighlight(Highlights.BlackTransparent);
			}
		}
	}

	private void UseMyGameObject(HexCell cell)
	{
		if (Active.NkmObject.GetType() == typeof(Character))
		{
			if (Active.Turn.WasCharacterPlaced)
			{
				throw new Exception("W tej turze już była wystawiona postać!");
			}

			var activeCharacter = Active.NkmObject as Character;
			try
			{
				_spawner.TrySpawning(cell, activeCharacter);
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
				Active.NkmObject = null;
				HexMapDrawer.RemoveHighlights();
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
	private void PlaceAllCharactersOnSpawns()
	{
		Players.ForEach(p => p.Characters.ForEach(c => TrySpawning(p, c)));
		if(Active.Phase.Number==0) Active.Phase.Finish();
	}

	private static void TrySpawning(GamePlayer p, Character c)
	{
		HexCell spawnPoint = p.GetSpawnPoints().FirstOrDefault(cell => cell.CharacterOnCell == null);
		if (spawnPoint == null) return;

		Spawner.Instance.TrySpawning(spawnPoint, c);
	}
}