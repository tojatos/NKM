using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using Hex;
using Managers;
using MyGameObjects.MyGameObject_templates;
using UIManagers;

namespace MyGameObjects.Abilities.Aqua
{
	public class Resurrection : Ability
	{
		private Character _characterToResurrect;
		public override List<HexCell> GetRangeCells()
		{
			return Active.GamePlayer.GetSpawnPoints().Where(sp => sp.CharacterOnCell == null).ToList();
		}

		public Resurrection()
		{
			Name = "Resurrection";
			Cooldown = 8;
			CurrentCooldown = 0;
			Type = AbilityType.Ultimatum;
			_characterToResurrect = null;
		}
		public override string GetDescription() => $@"{ParentCharacter.Name} wskrzesza sojuszniczą postać, która zginęła maksymalnie turę wcześniej.
Postać odradza się z połową maksymalnego HP, na wybranym spawnie.
Czas odnowienia: {Cooldown}";

		protected override void CheckIfCanBePrepared()
		{
			base.CheckIfCanBePrepared();
			if(!ParentCharacter.Owner.Characters.Any(c=>!c.IsAlive&&c.DeathTimer<=1))
				throw new Exception("Nie ma postaci do ożywienia!");
			if (GetRangeCells().Count == 0)
				throw new Exception("Nie ma miejsca na spawnach!");
		}
		protected override void Use()
		{
			Active.Ability = this;
			SpriteSelect.Instance.Open(Active.GamePlayer.Characters.Where(c => !c.IsAlive && c.DeathTimer <= 1),
				() =>
				{
					var selectedObj = SpriteSelect.Instance.SelectedObjects;
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

		public override void OnPhaseFinish()
		{
			base.OnPhaseFinish();
			_characterToResurrect = null;
		}

		//public override void Cancel()
		//{
		//	if(_characterToResurrect!=null) return;
		//	OnFailedUseFinish();
		//}
	}
}
