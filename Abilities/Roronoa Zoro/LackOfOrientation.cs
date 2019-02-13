﻿using System.Collections.Generic;
using System.Linq;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Roronoa_Zoro
{
	public class LackOfOrientation : Ability
	{
		public event Delegates.Void AfterGettingLost;
		
		public LackOfOrientation(Game game) : base(game, AbilityType.Passive, "Lack Of Orientation")
		{
			OnAwake += () => ParentCharacter.BasicMove = MoveOverride;
		}
		
		public override string GetDescription() => 
$"{ParentCharacter.Name} ma 50% szansy na pójście w losowe miejsce podczas wykonania ruchu.";
		
		private void MoveOverride(List<HexCell> moveCells)
		{
			bool isLost = NKMRandom.Get(Name, 0, 2) == 0;
			if (!isLost) ParentCharacter.DefaultBasicMove(moveCells);
			else
			{
				Active.RemoveMoveCells();
				int movementPoints = ParentCharacter.Speed.Value;
				Active.MoveCells.Add(ParentCharacter.ParentCell);
				HexCell lastCell = ParentCharacter.ParentCell;
				List<HexCell> moveTargets = ParentCharacter.GetBasicMoveCells();
				while (movementPoints-- > 0 || !lastCell.IsFreeToStand)
				{
					List<HexCell> neighborMoveCells = lastCell.GetNeighbors(Owner.Owner, 1).Intersect(moveTargets).ToList();
					lastCell = neighborMoveCells.GetRandom();
					Active.AddMoveCell(lastCell);
				}
				ParentCharacter.DefaultBasicMove(Active.MoveCells);
				Console.Log($"{ParentCharacter.FormattedFirstName()}: Cholera, znowu się zgubili?");
				AfterGettingLost?.Invoke();
			}
		}
	}
}
