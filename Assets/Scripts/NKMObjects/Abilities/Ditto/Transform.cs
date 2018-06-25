using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Ditto
{
    public class Transform : Ability, IClickable
    {
        private const int Range = 15;
        public Transform() : base(AbilityType.Normal, "Ditto")
        {
        }

        public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);

        public override string GetDescription() =>
            $"{ParentCharacter.Name} kopiuje statystyki (oprócz HP) i umiejętności wybranego wroga.\nZasięg: {Range}";

        public void Click()
        {
            throw new System.NotImplementedException();
        }
    }
}