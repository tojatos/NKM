using System.Collections.Generic;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Ditto
{
    public class Transform : Ability, IClickable
    {
        private const int Range = 15;
        public Transform(Game game) : base(game, AbilityType.Normal, "Ditto")
        {
        }

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner);

        public override string GetDescription() =>
            $"{ParentCharacter.Name} kopiuje statystyki (oprócz HP) i umiejętności wybranego wroga.\nZasięg: {Range}";

        public void Click()
        {
            throw new System.NotImplementedException();
        }
    }
}