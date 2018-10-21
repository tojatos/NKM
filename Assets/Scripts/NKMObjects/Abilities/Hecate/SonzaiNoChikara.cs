using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Hecate
{
	public class SonzaiNoChikara : Ability, IClickable, IUseable
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

		public void Use(List<HexCell> cells)
		{
            ItadakiNoKura passiveAbility = ParentCharacter.Abilities.OfType<ItadakiNoKura>().SingleOrDefault();

            if (passiveAbility != null)
            {
//                    List<Character> enemyCharactersOnMap = Game.Players.Where(p => p != Active.GamePlayer).SelectMany(p => p.Characters).Where(c => c.IsOnMap).ToList();
                List<NKMCharacter> enemyCharactersOnMap = GetTargetsInRange().GetCharacters();
                int damageValue = passiveAbility.CollectedEnergy / enemyCharactersOnMap.Count;
                enemyCharactersOnMap.ForEach(c =>
                {
                    var damage = new Damage(damageValue, DamageType.Magical);
                    ParentCharacter.Attack(this, c, damage);
                });
                passiveAbility.CollectedEnergyCharacters.Clear();
            }

            Finish();
		}
	}
}
