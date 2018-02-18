using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using Hex;
using Multiplayer.Network;
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
	public GameType Type { get; private set; }

	public Client Client;
	public Server Server;

	public bool IsInitialized;
	public Game()
	{
		Active = new Active(this);
	}

	public void Init(GameOptions gameOptions)
	{
		_options = gameOptions;

		Type = _options.GameType;
		Players = new List<GamePlayer>(gameOptions.Players);
		UIManager = _options.UIManager;
		HexMapDrawer = HexMapDrawer.Instance;
		Spawner = Spawner.Instance;
		Spawner.Init(this);

		Client = _options.Client;
		Server = _options.Server;

//		Players.ForEach(p => Debug.Log(p.Characters.Count));
		Debug.Log("Game started!");
		IsInitialized = true;
	}

	public void StartGame()
	{
		HexMapDrawer.CreateMap(_options.Map);
		UIManager.Init();
		UIManager.VisibleUI = UIManager.GameUI;
		Active.Buttons = UIManager.UseButtons;
		MainCameraController.Instance.Init();

		CharacterAbilities.Instance.Init();
//		Players.ForEach(p=>p.Init(this));
		UIManager.UpdateActivePhaseText();
		TakeTurns();
	}

	private async void TakeTurns()
	{
		while (true)
		{
			foreach (var player in Players)
			{
				Active.Turn.Start(player);
				Func<bool> isTurnDone = () => Active.Turn.IsDone;
				await isTurnDone.WaitToBeTrue();
			}
			//Skip finishing phase, if not every character is placed in the first phase
			if (Active.Phase.Number == 0 && Players.Any(p => p.Characters.Any(c => !c.IsOnMap)))
			{
				continue;
			}

			if (Players.All(p => p.Characters.Where(c => c.IsOnMap).All(c => !c.CanTakeAction)))
			{
				Active.Phase.Finish();
			}
		}
	}
	public void TryTouchingCell(HexCell touchedCell)
	{
		if (Type == GameType.MultiplayerClient)
		{
			Client.SendTouchCellMessage(touchedCell);
			return;
		}

		TouchCell(touchedCell);
	}

	public void TouchCell(HexCell touchedCell)
	{
		if (Active.MyGameObject != null)
		{
			//			switch (Type)
			//			{
			//				case GameType.Local:
			//				case GameType.MultiplayerServer:
			//					UseMyGameObject(touchedCell);
			//					break;
			//				case GameType.MultiplayerClient:
			//					Client.SendUseMyGameObjectMessage(touchedCell, Active.MyGameObject);
			//					break;
			//				default:
			//					throw new ArgumentOutOfRangeException();
			//			}
			UseMyGameObject(touchedCell);

		}
		else if (Active.HexCells?.Contains(touchedCell) == true)
		{
			//			switch (Type)
			//			{
			//				case GameType.Local:
			//				case GameType.MultiplayerServer:
			//					Active.MakeAction(touchedCell);
			//					break;
			//				case GameType.MultiplayerClient:
			//					Client.SendMakeActionMessage(touchedCell);
			//					break;
			//				default:
			//					throw new ArgumentOutOfRangeException();
			//			}
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
				Debug.Log(touchedCell.CharacterOnCell.Guid);
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
		var myGameObjectType = Active.MyGameObject.GetType().BaseType;
		if (myGameObjectType == typeof(Character))
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