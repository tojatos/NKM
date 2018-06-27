using System.Collections.Generic;
using System.Linq;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Bezimienni
{
    public class Check : Ability, IClickable, IUseable
    {
        public Check() : base(AbilityType.Normal, "Check", 2)
        {
	        OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }
        public override string GetDescription() => "Bezimienni szachują wybranego przeciwnika, wymuszając jego ruch.\nSzachowany wróg nie może użyć podstawowego ataku.";

	    public override List<HexCell> GetRangeCells() => new List<HexCell>(HexMapDrawer.Instance.Cells);
	    public override List<HexCell> GetTargetsInRange() => GetRangeCells().Where(c => c.CharacterOnCell != null && c.CharacterOnCell.Owner != ParentCharacter.Owner && c.CharacterOnCell.TookActionInPhaseBefore == false).ToList();

	    public void Click() => Active.Prepare(this, GetTargetsInRange());
	    public void Use(List<HexCell> cells) => Use(cells[0].CharacterOnCell);

	    private void Use(Character character)
		{
			Turn.PlayerDelegate forceAction = null;
			forceAction = (player) =>
			{
				if (player != character.Owner) return;
				Active.Turn.CharacterThatTookActionInTurn = character;
				Active.Turn.TurnStarted -= forceAction;
			};
			Active.Turn.TurnStarted += forceAction;
              character.Effects.Add(new BasicAttackInability(1, character, Name));
			Finish();
		}

    }
}
