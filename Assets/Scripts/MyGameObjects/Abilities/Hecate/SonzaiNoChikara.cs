using System;
using System.Collections.Generic;
using System.Linq;
using Hex;
using Managers;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Hecate
{
	public class SonzaiNoChikara : Ability
	{
		public SonzaiNoChikara()
		{
			Name = "Sonzai no Chikara";
			Cooldown = 8;
			CurrentCooldown = 0;
			Type = AbilityType.Ultimatum;
		}

		public override List<HexCell> GetRangeCells()
		{
			return Game.HexMapDrawer.Cells;
		}

		public override string GetDescription()
		{
			return string.Format(
@"{0} uwalnia zgromadzoną Energię Życiową, raniąc każdego wroga na mapie.
Ilość HP, jakie zgromadziła w postaci Energii Życiowej jest równo rozdzielana pomiędzy wszystkich przeciwników w postaci obrażeń magicznych.",
				ParentCharacter.Name);
		}
		protected override void Use()
		{
			var cellRange = GetRangeCells();
			Active.Prepare(this, cellRange);
			Active.MakeAction(cellRange);
		}

		public override void Use(List<Character> characters)
		{
			try
			{
				var passiveAbility = ParentCharacter.Abilities.OfType<ItadakiNoKura>().SingleOrDefault();
				if (passiveAbility == null) throw new Exception("Pasywna umiejętność nie znaleziona!");

				var enemyCharactersOnMap = Game.Players.Where(p => p != Active.Player).SelectMany(p => p.Characters).Where(c => c.IsOnMap).ToList();
				var damage = passiveAbility.CollectedEnergy / enemyCharactersOnMap.Count;
				//Animations.Instance.SonzaiNoChikara(enemyCharactersOnMap.Select(c=>c.CharacterObject.transform).ToList());
				enemyCharactersOnMap.ForEach(c => ParentCharacter.Attack(c, AttackType.Magical, damage));

				passiveAbility.CollectedEnergyCharacters.Clear();
				OnUseFinish();
			}
			catch (Exception e)
			{
				MessageLogger.DebugLog(e.Message);
				OnFailedUseFinish();
			}
		}
	}
}
