using System;
using System.Collections.Generic;
using Helpers;
using Hex;
using MyGameObjects.Effects;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Bezimienni
{
    public class Check : Ability
    {
        public Check()
        {
            Name = "Check";
            Cooldown = 2;
            CurrentCooldown = 0;
            Type = AbilityType.Normal;
        }
        public override string GetDescription()
        {
	        return
		        "Bezimienni szachują wybranego przeciwnika, wymuszając jego ruch.\nSzachowany wróg nie może użyć podstawowego ataku.";
        }

	    public override List<HexCell> GetRangeCells() => new List<HexCell>(HexMapDrawer.Instance.Cells);
	    
        protected override void CheckIfCanBePrepared()
		{
			base.CheckIfCanBePrepared();
//			List<HexCell> cellRange = GetRangeCells();
//			cellRange.RemoveNonEnemies();
//			if (cellRange.Count == 0)
//			{
//				throw new Exception("Nie ma nikogo w zasięgu umiejętności!");
//			}TODO
		}
	    
//		protected override void Use()
//		{
//			List<HexCell> cellRange = GetRangeCells();
//			Active.Prepare(this, cellRange);
//			Active.MakeAction(cellRange);
//		}
//		public override void Use(List<HexCell> cells)
//		{
//			List<Character> characters = cells.GetCharacters();
//			characters.ForEach(c =>
//			{
//				c.Effects.Add(new Stun(1, c, Name)); 
//				c.Effects.Add(new StatModifier(2, -3, c, StatType.Speed, Name));
//			});
//			OnUseFinish();
//		}
//TODO

    }
}