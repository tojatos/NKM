using System.Collections.Generic;
using NKMCore.Effects;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Carmel_Wilhelmina
{
    public class TiamatsIntervention : Ability, IClickable, IUseableCell, IUseableCharacter
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

        private Character _selectedCharacter;

        public void Use(Character character)
        {
            _selectedCharacter = character;
            Active.Prepare(this, GetMoveTargets());
        }

        public void Use(HexCell cell)
        {
            ParentCharacter.TryToTakeTurn();
            _selectedCharacter.MoveTo(cell);
            if (_selectedCharacter.IsEnemyFor(Owner)) _selectedCharacter.Effects.Add(new Stun(Game, StunDuration, _selectedCharacter, Name));
            else _selectedCharacter.Shield.Value += Shield;

            Finish(); 
        }
    }
}