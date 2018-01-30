using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Helpers;
using Hex;
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

		Players.ForEach(p => Debug.Log(p.Characters.Count));
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
//					while (!Active.Turn.IsDone)
//					{
//						await Task.Delay(1);
//					}
			}
			//Skip finishing phase, if not every character is placed in the first phase
//			if (Active.Phase.Number == 0 && Players.Any(p => p.Characters.Any(c => !c.IsOnMap)))
//			{
//				continue;
//			}

			if (Players.All(p => p.Characters.Where(c => c.IsOnMap).All(c => !c.CanTakeAction)))
			{
				Active.Phase.Finish();
			}
		}
	}
	public void TouchCell(HexCell touchedCell)
	{

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
			}
			else
			{
				Active.CharacterOnMap?.Deselect();
				touchedCell.ToggleHighlight();
			}
		}
	}

	private void UseMyGameObject(HexCell cell)
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
		//else if (myGameObjectType == typeof(Item))
		//{
		//	if (cell.CharacterOnCell == null) return;
		//	if (cell.CharacterOnCell.Owner != Active.GamePlayer)
		//	{
		//		throw new Exception("Nie możesz użyć przedmiotu na nie swojej postaci!");
		//	}
		//	var activeItem = Active.MyGameObject as Item;
		//	cell.CharacterOnCell.ActiveItem = activeItem;
		//	Active.GamePlayer.Items.Remove(activeItem);
		//	Active.MyGameObject = null;
		//}
		//else if (myGameObjectType == typeof(Potion))
		//{
		//}
	}
}