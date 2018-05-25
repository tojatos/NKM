using System.Collections.Generic;
using Hex;
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
            return "";
        }
		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Radius);
    }
}