using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;
using NKMObject = NKMObjects.Templates.NKMObject;

namespace NKMObjects.Abilities.Aqua
{
	public class Resurrection : Ability, IClickable, IUseable
	{
		private NKMCharacter _characterToResurrect;
		public override List<HexCell> GetRangeCells() => Active.GamePlayer.GetSpawnPoints().Where(sp => sp.CharacterOnCell == null).ToList();

		public Resurrection() : base(AbilityType.Ultimatum, "Resurrection", 8)
		{
			OnAwake += () =>
			{
                Validator.ToCheck.Add(() => IsAnyCharacterToRevive);
                Validator.ToCheck.Add(() => IsAnyFreeCellToPlaceACharacter);
//				Active.Phase.PhaseFinished += () => _characterToResurrect = null; //TODO: is this really needed?
			};
		}
		public override string GetDescription() => 
$@"{ParentCharacter.Name} wskrzesza sojuszniczą postać, która zginęła maksymalnie fazę wcześniej.
Postać odradza się z połową maksymalnego HP, na wybranym spawnie.
Czas odnowienia: {Cooldown}";

		private bool IsAnyCharacterToRevive => ParentCharacter.Owner.Characters.Any(c => !c.IsAlive && c.DeathTimer <= 1);
		private bool IsAnyFreeCellToPlaceACharacter => GetRangeCells().Count >= 1;

		public void Click()
		{
//			Active.AbilityToUse = this;
			Active.Prepare(this);
			SpriteSelect.Instance.Open(Active.GamePlayer.Characters.Where(c => !c.IsAlive && c.DeathTimer <= 1),
				FinishSelectingButtonClick, "Postać do ożywienia", "Zakończ wybieranie postaci");
		}

		private void FinishSelectingButtonClick()
		{
			List<NKMObject> selectedObj = SpriteSelect.Instance.SelectedObjects;
			if (selectedObj.Count != 1) return;

			var character = (NKMCharacter) selectedObj[0];
			_characterToResurrect = character;
			Active.Prepare(this, GetRangeCells());
			SpriteSelect.Instance.Close();
		}

		private void Use(HexCell cell)
		{
				Spawner.Instance.Spawn(cell, _characterToResurrect);
				_characterToResurrect.HealthPoints.Value = _characterToResurrect.HealthPoints.BaseValue / 2;

				Finish();
			
		}
		public void Use(List<HexCell> cells) => Use(cells[0]);
	}
}
