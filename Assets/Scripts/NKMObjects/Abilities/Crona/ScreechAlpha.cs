using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Crona
{
    public class ScreechAlpha : Ability, IClickable, IUseable
    {
        private const int Radius = 3;

	    public ScreechAlpha() : base(AbilityType.Normal, "Screech Alpha", 4)
	    {
		    OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
	    }
	    
	    public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Radius);
	    public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);
	    
        public override string GetDescription() => 
@"Miecz Crony, Ragnarok, wydaje z siebie krzyk,
ogłuszający wrogów dookoła na 1 turę i spowalniający ich na 1 następną.";

	    public void Click()
		{
			Active.Prepare(this, GetTargetsInRange());
			Active.MakeAction(Active.HexCells);
		}
		public void Use(List<HexCell> cells)
		{
			List<Character> characters = cells.GetCharacters();
			characters.ForEach(c =>
			{
				c.Effects.Add(new Stun(1, c, Name)); 
				c.Effects.Add(new StatModifier(2, -3, c, StatType.Speed, Name));
			});
			Finish();
		}


    }
}