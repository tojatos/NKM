using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;
using NKMObject = NKMObjects.Templates.NKMObject;

namespace NKMObjects.Abilities.Aqua
{
	public class Resurrection : Ability, IClickable
	{
		private Character _characterToResurrect;
		public override List<HexCell> GetRangeCells() => Active.GamePlayer.GetSpawnPoints().Where(sp => sp.CharacterOnCell == null).ToList();

		public Resurrection() : base(AbilityType.Ultimatum, "Resurrection", 8)
		{
			OnAwake += () =>
			{
                Validator.ToCheck.Add(() => IsAnyCharacterToRevive);
                Validator.ToCheck.Add(() => IsAnyFreeCellToPlaceACharacter);
				Active.Phase.PhaseFinished += () => _characterToResurrect = null; //TODO: is this really needed?
			};
		}
		public override string GetDescription() => $@"{ParentCharacter.Name} wskrzesza sojuszniczą postać, która zginęła maksymalnie turę wcześniej.
Postać odradza się z połową maksymalnego HP, na wybranym spawnie.
Czas odnowienia: {Cooldown}";

		private bool IsAnyCharacterToRevive => ParentCharacter.Owner.Characters.Any(c => !c.IsAlive && c.DeathTimer <= 1);
		private bool IsAnyFreeCellToPlaceACharacter => GetRangeCells().Count >= 1;

		public void Click()
		{
			Active.Ability = this;
			SpriteSelect.Instance.Open(Active.GamePlayer.Characters.Where(c => !c.IsAlive && c.DeathTimer <= 1),
				() =>
				{
					List<NKMObject> selectedObj = SpriteSelect.Instance.SelectedObjects;
					if (selectedObj.Count != 1) return;

					Use((Character)selectedObj[0]);
					SpriteSelect.Instance.Close();
				}, "Postać do ożywienia", "Zakończ wybieranie postaci");
		}
		public override void Use(Character character)
		{
			_characterToResurrect = character;
			Active.Prepare(this, GetRangeCells());
		}
		public override void Use(HexCell cell)
		{
			try
			{
				Spawner.Instance.TrySpawning(cell, _characterToResurrect);
				_characterToResurrect.HealthPoints.Value = _characterToResurrect.HealthPoints.BaseValue / 2;

				OnUseFinish();
			}
			catch (Exception e)
			{
				MessageLogger.DebugLog(e.Message);
				throw;
			}
		}
	}
}
