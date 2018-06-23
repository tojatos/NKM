using System.Collections.Generic;
using System.Linq;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Bezimienni
{
    public class Check : Ability, IClickable
    {
        public Check() : base(AbilityType.Normal, "Check", 2)
        {
	        OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }
        public override string GetDescription() => "Bezimienni szachują wybranego przeciwnika, wymuszając jego ruch.\nSzachowany wróg nie może użyć podstawowego ataku.";

	    public override List<HexCell> GetRangeCells() => new List<HexCell>(HexMapDrawer.Instance.Cells);
	    public override List<HexCell> GetTargetsInRange() => GetRangeCells().Where(c => c.CharacterOnCell != null && c.CharacterOnCell.Owner != ParentCharacter.Owner && c.CharacterOnCell.TookActionInPhaseBefore == false).ToList();

//        protected override void CheckIfCanBePrepared()
//		{
//			base.CheckIfCanBePrepared();
//			List<HexCell> cellRange = GetRangeCells().Where(c => c.CharacterOnCell != null && c.CharacterOnCell.Owner != ParentCharacter.Owner && c.CharacterOnCell.TookActionInPhaseBefore == false).ToList();
//			if (cellRange.Count == 0)
//			{
//				throw new Exception("Nie ma nikogo w zasięgu umiejętności!");
//			}
//		}


	    public void Click()
		{
//			List<HexCell> cellRange = GetRangeCells().Where(c => c.CharacterOnCell != null && c.CharacterOnCell.Owner != ParentCharacter.Owner && c.CharacterOnCell.TookActionInPhaseBefore == false).ToList();
//			Active.Prepare(this, cellRange);
//			Active.MakeAction(cellRange);
			Active.Prepare(this, GetTargetsInRange());
		}
		public override void Use(Character character)
		{
			Turn.PlayerDelegate forceAction = null;
			forceAction = (player) =>
			{
				if (player != character.Owner) return;
				Active.Turn.CharacterThatTookActionInTurn = character;
				Active.Turn.TurnStarted -= forceAction;
			};
			Active.Turn.TurnStarted += forceAction;
//			Active.Turn.TurnStarted += (player) => Active.Turn.TurnStarted -= forceAction;
      character.Effects.Add(new BasicAttackInability(1, character, Name));
			OnUseFinish();
		}

    }
}
