using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Hecate
{
	public class SonzaiNoChikara : Ability, IClickable
	{
		public SonzaiNoChikara() : base(AbilityType.Ultimatum, "Sonzai no Chikara", 8){}

		public override List<HexCell> GetRangeCells() => Game.HexMapDrawer.Cells;
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);

		public override string GetDescription() => 
$@"{ParentCharacter.Name} uwalnia zgromadzoną Energię Życiową, raniąc każdego wroga na mapie.
Ilość HP, jakie zgromadziła w postaci Energii Życiowej jest równo rozdzielana pomiędzy wszystkich przeciwników w postaci obrażeń magicznych.";

		public void Click()
		{
			Active.Prepare(this, GetTargetsInRange());
			Active.MakeAction(Active.HexCells);
		}

		public override void Use(List<HexCell> cells)
		{
			try
			{
				ItadakiNoKura passiveAbility = ParentCharacter.Abilities.OfType<ItadakiNoKura>().SingleOrDefault();
				if (passiveAbility == null) throw new Exception("Pasywna umiejętność nie znaleziona!");

				List<Character> enemyCharactersOnMap = Game.Players.Where(p => p != Active.GamePlayer).SelectMany(p => p.Characters).Where(c => c.IsOnMap).ToList();
				var damageValue = passiveAbility.CollectedEnergy / enemyCharactersOnMap.Count;
				//Animations.Instance.SonzaiNoChikara(enemyCharactersOnMap.Select(c=>c.CharacterObject.transform).ToList());
				enemyCharactersOnMap.ForEach(c =>
				{
                    var damage = new Damage(damageValue, DamageType.Magical);
					ParentCharacter.Attack(c, damage);
				});

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
