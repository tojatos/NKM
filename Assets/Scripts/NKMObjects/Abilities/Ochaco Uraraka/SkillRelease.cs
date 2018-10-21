using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Ochaco_Uraraka
{
    public class SkillRelease : Ability, IClickable
    {
        private const int Duration = 2;
        public SkillRelease() : base(AbilityType.Ultimatum, "Skill Release", 4)
        {
            OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
        }

        public override List<HexCell> GetRangeCells() => HexMapDrawer.Instance.Cells;
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyCharacters().FindAll(c => c.CharacterOnCell.Effects.Any(e => e.Name == "Zero Gravity"));

        public override string GetDescription() =>
$@"{ParentCharacter.FirstName()} uwalnia podstawową umiejętność,
sprawiając że wszystkie postacie tracą efekt Zero Gravity,
a przeciwnicy, którzy stracili ten efekt zostają ogłuszeni na {Duration} fazy.";

        public void Click()
        {
            Active.MakeAction();
            List<NKMCharacter> targets = GetTargetsInRange().GetCharacters();
            targets.ForEach(c => c.Effects.FindAll(e => e.Name == "Zero Gravity").ForEach(ef => ef.RemoveFromParent()));
            targets.FindAll(t => t.Owner != Owner).Distinct().ToList().ForEach(c => c.Effects.Add(new Stun(Duration, c, Name)));
            
            Finish();
        }

    }
}