﻿using System;
using System.Collections.Generic;
using Helpers;
using Hex;
using MyGameObjects.Effects;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Crona
{
    public class ScreechAlpha : Ability
    {
        private const int Radius = 3;
        public ScreechAlpha()
        {
            Name = "Screech Alpha";
            Cooldown = 4;
            CurrentCooldown = 0;
            Type = AbilityType.Normal;
        }
        public override string GetDescription()
        {
            return "Miecz Crony, Ragnarok, wydaje z siebie krzyk,\nogłuszający wrogów dookoła na 1 turę i spowalniający ich na 1 następną.";
        }
		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Radius);
	    
        protected override void CheckIfCanBePrepared()
		{
			base.CheckIfCanBePrepared();
			List<HexCell> cellRange = GetRangeCells();
			cellRange.RemoveNonEnemies();
			if (cellRange.Count == 0)
			{
				throw new Exception("Nie ma nikogo w zasięgu umiejętności!");
			}
		}
	    
		protected override void Use()
		{
			List<HexCell> cellRange = GetRangeCells();
			Active.Prepare(this, cellRange);
			Active.MakeAction(cellRange);
		}
		public override void Use(List<Character> characters)
		{
			characters.ForEach(c =>
			{
				c.Effects.Add(new Stun(1, c, Name)); 
				c.Effects.Add(new StatModifier(2, -3, c, StatType.Speed, Name));
			});
			OnUseFinish();
		}


    }
}