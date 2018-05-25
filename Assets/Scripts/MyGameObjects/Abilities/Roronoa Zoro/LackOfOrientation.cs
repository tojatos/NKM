﻿using System.Collections.Generic;
using Helpers;
using Hex;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Roronoa_Zoro
{
	public class LackOfOrientation : Ability
	{
		public LackOfOrientation()
		{
			Name = "Lack Of Orientation";
			Type = AbilityType.Passive;
			OverridesMove = true;
		}

		public override string GetDescription() => $"{ParentCharacter.Name} ma 50% szansy na pójście w losowe miejsce podczas wykonania ruchu.";

		public override void Move(List<HexCell> moveCells)
		{
			if (UnityEngine.Random.Range(0,2) == 0)
			{
				ParentCharacter.BasicMove(moveCells);
			}
			else
			{
				Active.RemoveMoveCells();
				var movementPoints = ParentCharacter.Speed.Value;
				Active.MoveCells.Add(ParentCharacter.ParentCell);
				HexCell lastCell = ParentCharacter.ParentCell; 
				while (movementPoints-- != 0)
				{
					List<HexCell> neighborMoveCells = lastCell.GetNeighbors(1, true, true);
                    neighborMoveCells.RemoveAll(cell => cell.CharacterOnCell != null); //we don't want to allow stepping into our characters!
					var r = UnityEngine.Random.Range(0, neighborMoveCells.Count);
					lastCell = neighborMoveCells[r];
					Active.AddMoveCell(lastCell);
				}
				ParentCharacter.BasicMove(Active.MoveCells);
				MessageLogger.Log($"{ParentCharacter.FormattedFirstName()}: Cholera, znowu się zgubili?");
			}
		}
	}
}
