using System.Collections.Generic;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Carmel_Wilhelmina
{
    public class TiamatsIntervention : Ability, IClickable, IUseable
    {
        private const int Range = 8;
        private const int MoveTargetRange = 3;
        private const int Shield = 10;
        private const int StunDuration = 1;
        public TiamatsIntervention(Game game) : base(game, AbilityType.Ultimatum, "Tiamat's Intervention", 6)
        {
            OnAwake += () => Validator.ToCheck.Add(()=>GetMoveTargets().Count > 0);
        }

        public override string GetDescription() =>
$@"{ParentCharacter.Name} przyciąga do siebie jednostkę znajdującą się w promieniu {Range} w miejsce oddalone maksymalnie {MoveTargetRange} pola od niej.
Dodatkowo, jeśli jest to sojusznik, to daje mu {Shield} tarczy,
a jeśli przeciwnik, ogłusza go na {StunDuration} fazę.";

        public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);
        public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereCharacters();

        private List<HexCell> GetMoveTargets() =>
            GetNeighboursOfOwner(MoveTargetRange).FindAll(e => e.IsFreeToStand);

        public void Click() => Active.Prepare(this, GetTargetsInRange());

        public void Use(List<HexCell> cells)
        {
            HexCell cell = cells[0];
            if(!cell.IsEmpty) Use(cell.CharactersOnCell[0]);
            else Use(cell);
        }

        private Character _selectedCharacter;

        private void Use(Character character)
        {
            _selectedCharacter = character;
            Active.Prepare(this, GetMoveTargets());
        }

        private void Use(HexCell cell)
        {
            _selectedCharacter.MoveTo(cell);
            if (_selectedCharacter.IsEnemyFor(Owner)) _selectedCharacter.Effects.Add(new Stun(Game, StunDuration, _selectedCharacter, Name));
            else _selectedCharacter.Shield.Value += Shield;

            Finish(); 
        }
    }
}