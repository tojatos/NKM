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

		public override void Move(HexCell cell)
		{
			if (UnityEngine.Random.Range(0,2) == 0)
			{
				ParentCharacter.BasicMove(cell);
			}
			else
			{
				var r = UnityEngine.Random.Range(0, Active.HexCells.Count);
				var randomCell = Active.HexCells[r];
				ParentCharacter.BasicMove(randomCell);
				MessageLogger.Log(string.Format("{0}: Cholera, znowu się zgubili?", ParentCharacter.FormattedFirstName));
			}
		}
	}
}
