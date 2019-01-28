using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;
//using NKMObject = NKMObjects.Templates.NKMObject;

namespace NKMObjects.Abilities.Aqua
{
	public class Resurrection : Ability, IClickable, IUseable
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
			Active.Prepare(this);
			SpriteSelect.Instance.Open(Active.GamePlayer.Characters.Where(_isResurrectable),
				FinishSelectingButtonClick, "Postać do ożywienia", "Zakończ wybieranie postaci");
		}

		private void FinishSelectingButtonClick()
		{
			List<Character> selectedObj = SpriteSelect.Instance.SelectedObjects;
			if (selectedObj.Count != 1) return;

			Character character = selectedObj[0];
			_characterToResurrect = character;
			Active.Prepare(this, GetRangeCells());
			SpriteSelect.Instance.Close();
		}

		private void Use(HexCell cell)
		{
			Game.HexMap.Place(_characterToResurrect, cell);
            _characterToResurrect.HealthPoints.Value = _characterToResurrect.HealthPoints.BaseValue / 2;

            Finish();
			
		}
		public void Use(List<HexCell> cells) => Use(cells[0]);
	}
}
