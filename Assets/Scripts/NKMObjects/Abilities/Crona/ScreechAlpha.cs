using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Crona
{
    public class ScreechAlpha : Ability, IClickable
    {
        private const int Radius = 3;

	    public ScreechAlpha(Game game) : base(game, AbilityType.Normal, "Screech Alpha", 4)
	    {
		    OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
	    }
	    
	    public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Radius);
	    public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner);
	    
        public override string GetDescription() => 
@"Miecz Crony, Ragnarok, wydaje z siebie krzyk,
ogłuszający wrogów dookoła na 1 turę i spowalniający ich na 1 następną.";

	    public void Click()
		{
			ParentCharacter.TryToTakeTurn();
			GetTargetsInRange().GetCharacters().ForEach(c =>
			{
				c.Effects.Add(new Stun(Game, 1, c, Name)); 
				c.Effects.Add(new StatModifier(Game, 2, -3, c, StatType.Speed, Name));
			});
			Finish();
		}
    }
}