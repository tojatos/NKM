using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Roronoa_Zoro
{
	public class LackOfOrientation : Ability
	{
		public LackOfOrientation() : base(AbilityType.Passive, "Lack Of Orientation")
		{
			OnAwake += () => ParentCharacter.BasicMove = MoveOverride;
		}
		
		public override string GetDescription() => 
$"{ParentCharacter.Name} ma 50% szansy na pójście w losowe miejsce podczas wykonania ruchu.";
		
		private void MoveOverride(List<HexCell> moveCells)
		{
			bool isLost = UnityEngine.Random.Range(0, 2) == 0;
			if (!isLost) ParentCharacter.DefaultBasicMove(moveCells);
			else
			{
				Active.RemoveMoveCells();
				var movementPoints = ParentCharacter.Speed.Value;
				Active.MoveCells.Add(ParentCharacter.ParentCell);
				HexCell lastCell = ParentCharacter.ParentCell;
				List<HexCell> moveTargets = ParentCharacter.GetBasicMoveCells();
				while (movementPoints-- > 0 || !lastCell.IsFreeToStand)
				{
//					List<HexCell> neighborMoveCells = lastCell.GetNeighbors(1,
//						SearchFlags.StopAtEnemyCharacters | SearchFlags.StopAtFriendlyCharacters | SearchFlags.StopAtWalls);
					List<HexCell> neighborMoveCells = lastCell.GetNeighbors(1).Intersect(moveTargets).ToList();
					int r = UnityEngine.Random.Range(0, neighborMoveCells.Count);
					lastCell = neighborMoveCells[r];
					Active.AddMoveCell(lastCell);
				}
				ParentCharacter.DefaultBasicMove(Active.MoveCells);
				Console.Log($"{ParentCharacter.FormattedFirstName()}: Cholera, znowu się zgubili?");
				int rand = UnityEngine.Random.Range(1, 4);
				Active.PlayAudio("op wtf " + rand);
			}
		}
	}
}
