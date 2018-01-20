using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hex;
using MyGameObjects.MyGameObject_templates;
using UIManagers;
using UnityEngine;

namespace Managers
{
	public class GameManager : SingletonMonoBehaviour<GameManager>
	{
		public UIManager UIManager;
		public Spawner Spawner;
		private Active Active;

		private int NumberOfPlayers;
		public static List<Player> Players { get; private set; }
		public static int GetIndex(Player player) => Players.FindIndex(p=>p==player);

		private void Awake()
		{
			Active = Active.Instance;
			NumberOfPlayers = PlayerPrefs.GetInt("NumberOfPlayers", 2);
			var mapIndex = PlayerPrefs.GetInt("SelectedMap", 0);
			HexMapDrawer.Instance.HexMap = Stuff.Maps[mapIndex];
			CreatePlayers();
		}
		private void Start()
		{
			Active.UI = UIManager.GameUI;
			Active.Buttons = UIManager.UseButtons;
			StartCoroutine(StartGame());
		}
		private void CreatePlayers()
		{
			Players = new List<Player>();
			for (var i = 0; i < NumberOfPlayers; i++)
				Players.Add(new Player {Name = $"Player{i + 1}"});
		}
		private IEnumerator StartGame()
		{
			UIManager.UpdateActivePhaseText();
			//Game loop
			while (true)
			{
				foreach (var player in Players)
				{
					Active.Turn.Start(player);
					yield return new WaitUntil(() => Active.Turn.IsDone);
				}
				//Skip finishing phase, if not every character is placed in the first phase
				if (Active.Phase.Number == 0 && Players.Any(p => p.Characters.Any(c => !c.IsOnMap)))
				{
					continue;
				}

				if(Players.All(p => p.Characters.Where(c=>c.IsOnMap).All(c => !c.CanTakeAction)))
				{
					Active.Phase.Finish();
				}
			}
			// ReSharper disable once IteratorNeverReturns
		}

		private void Update()
		{
			if (Active.UI == UIManager.GameUI)
			{
				if (Active.AirSelection.IsEnabled)
				{
					var cellPointed = CellPointed();
					if (cellPointed != null && Active.HexCells.Contains(cellPointed))
					{
						Active.AirSelection.HexCells = new List<HexCell> { cellPointed };
					}
				}
				if (Input.GetMouseButtonDown(0))
				{
					if (Active.IsPointerOverUIObject()) return; //Do not touch cells if mouse is over UI

					var cellPointed = CellPointed();
					if (cellPointed != null)
					{
						if (Active.AirSelection.IsEnabled && Active.HexCells.Contains(cellPointed))
						{
							Active.MakeAction(Active.AirSelection.HexCells);
						}
						else
						{
							TouchCell(cellPointed);
						}
					}
				}

				if (Input.GetMouseButtonDown(1)|| Input.GetKeyDown(KeyCode.Escape))
				{
					Active.Cancel();
				}
			}
		}
		private HexCell CellPointed()
		{
			var inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (!Physics.Raycast(inputRay, out hit)) return null;

			var position = hit.point;
			return GetCellByPosition(ref position);
		}
		private void TouchCell(HexCell touchedCell)
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
					if (Active.CharacterOnMap != null)
					{
						Character.Deselect();
					}
					touchedCell.ToggleHighlight();
				}
			}
		}
		private HexCell GetCellByPosition(ref Vector3 position)
		{
			position = transform.InverseTransformPoint(position);
			var coordinates = HexCoordinates.FromPosition(position);
			var index = coordinates.X + coordinates.Z * HexMapDrawer.Instance.Width + coordinates.Z / 2;
			var touchedCell = HexMapDrawer.Cells[index];
			return touchedCell;
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
					TrySpawning(cell, activeCharacter);
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
			//	if (cell.CharacterOnCell.Owner != Active.Player)
			//	{
			//		throw new Exception("Nie możesz użyć przedmiotu na nie swojej postaci!");
			//	}
			//	var activeItem = Active.MyGameObject as Item;
			//	cell.CharacterOnCell.ActiveItem = activeItem;
			//	Active.Player.Items.Remove(activeItem);
			//	Active.MyGameObject = null;
			//}
			//else if (myGameObjectType == typeof(Potion))
			//{
			//}
		}
		public void TrySpawning(HexCell cell, Character characterToSpawn)
		{
			var playerSpawnpointType =
				HexMapDrawer.Instance.HexMap.SpawnPoints[Players.FindIndex(player => player == Active.Player)];
			if (cell.Type != playerSpawnpointType)
			{
				throw new Exception("To nie twój spawn!");
			}
			if (cell.CharacterOnCell != null)
			{
				throw new Exception("Tu już stoi postać zwana " + cell.CharacterOnCell.Name + "!");
			}

			Spawner.SpawnCharacterObject(cell, characterToSpawn);
		}

	}
}