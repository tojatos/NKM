using System;
using System.Collections.Generic;
using System.Linq;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;

namespace NKMCore.Abilities.Aqua
{
	public class Resurrection : Ability, IClickable, IUseableCell
	{
		private Character _characterToResurrect;
		private readonly Func<Character, bool> _isResurrectable = c => !c.IsAlive && c.DeathTimer <= 1;
		public override List<HexCell> GetRangeCells() => Active.GamePlayer.GetSpawnPoints(HexMap).Where(sp => sp.IsFreeToStand).ToList();

		public Resurrection(Game game) : base(game, AbilityType.Ultimatum, "Resurrection", 8)
		{
			OnAwake += () =>
			{
                Validator.ToCheck.Add(() => IsAnyCharacterToRevive);
                Validator.ToCheck.Add(() => IsAnyFreeCellToPlaceACharacter);
			};
		}
		public override string GetDescription() => 
$@"{ParentCharacter.Name} wskrzesza sojuszniczą postać, która zginęła maksymalnie fazę wcześniej.
Postać odradza się z połową maksymalnego HP, na wybranym spawnie.
Czas odnowienia: {Cooldown}";

		private bool IsAnyCharacterToRevive => ParentCharacter.Owner.Characters.Any(_isResurrectable);
		private bool IsAnyFreeCellToPlaceACharacter => GetRangeCells().Count >= 1;

		public void Click()
		{
			Active.Select(new SelectableProperties<Character>()
			{
				ToSelect = Active.GamePlayer.Characters.FindAll(_isResurrectable.Invoke),
				ConstraintOfSelection = list => list.Count == 1,
				OnSelectFinish = list =>
				{
					_characterToResurrect = list[0];
                    Active.Prepare(this, GetRangeCells());
				},
				SelectionTitle = "Postać do ożywienia",
			});
		}

		public void Use(HexCell cell)
		{
			ParentCharacter.TryToTakeTurn();
			Game.HexMap.Place(_characterToResurrect, cell);
            _characterToResurrect.HealthPoints.Value = _characterToResurrect.HealthPoints.BaseValue / 2;

            Finish();
		}
	}
}
