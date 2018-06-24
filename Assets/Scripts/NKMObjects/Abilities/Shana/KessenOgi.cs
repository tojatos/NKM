using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Shana
{
	public class KessenOgi : Ability, IClickable
	{
		private const int FlameDamage = 15;
		private const int Range = 5;
		private const int ShinkuKnockback = 2;
		private const int ShinpanAndDanzaiDamage = 5;
		private const int ShinpanAndDanzaiRange = 6;
		private const int FlameWidth = 1;

		public KessenOgi() : base(AbilityType.Ultimatum, "Kessen Ōgi", 4)
		{
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
		}
			
		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(Range, SearchFlags.StopAtWalls | SearchFlags.StraightLine);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);

		public override string GetDescription() => 
$@"{ParentCharacter.Name} używa kolejno po sobie występujących umiejętności:

<b>Shinku:</b>
Odpycha wroga o {ShinkuKnockback} pola w stronę, w którą był wykonany atak.

<b>Hien:</b>
Uderza falą płomieni za {FlameDamage}, fala, o szerokości {FlameWidth}, ma  zasięg do lini, w której stoi odepchnięty wróg.

<b>Shinpan + Danzai:</b>
Bije {ShinpanAndDanzaiDamage} nieuchronnych obrażeń celowi za każdą postać (poza sobą),
która jest w obszarze oddalonym od Shany o {ShinpanAndDanzaiRange}.
Zasięg użycia: {Range} Czas odnowienia: {Cooldown}";

		public void Click() => Active.Prepare(this, GetTargetsInRange());

		public override void Use(Character character)
		{
			Shinku(character);
			Hien(character);
			ShinpanAndDanzai(character);
			OnUseFinish();
		}
		private void Shinku(Character character)
		{
			HexDirection direction = ParentCharacter.ParentCell.GetDirection(character.ParentCell);
			HexCell lastCell = character.ParentCell;
			for (int i = ShinkuKnockback; i > 0; i--) {
				HexCell cell = lastCell.GetCell(direction, 1);
				if(cell==null || cell.CharacterOnCell != null || cell.Type == HexTileType.Wall) break;
				lastCell = cell;
			}
			if(lastCell!=ParentCharacter.ParentCell) character.MoveTo(lastCell);
		}
		private void Hien(Character character)
		{
			List<HexCell> cells = new List<HexCell>();
			HexDirection direction = ParentCharacter.ParentCell.GetDirection(character.ParentCell);
			HexCell lastCell = ParentCharacter.ParentCell;
			HexCell neighbor;
			do
			{
				neighbor = lastCell.GetCell(direction, 1);
				if(neighbor==null) break;
				cells.Add(neighbor);
				lastCell = neighbor;
			} while (neighbor != character.ParentCell);
			foreach (HexCell c in cells)
			{
				if (c.CharacterOnCell == null || c.CharacterOnCell.Owner == ParentCharacter.Owner) continue;
				var damage = new Damage(FlameDamage, DamageType.Magical);
				ParentCharacter.Attack(this, c.CharacterOnCell, damage);
			}
		}
		private void ShinpanAndDanzai(Character character)
		{
			int charactersNearParent = ParentCharacter.ParentCell.GetNeighbors(ShinpanAndDanzaiRange).Count(c => c.CharacterOnCell != null);
			var damageValue = ShinpanAndDanzaiDamage * charactersNearParent;
			var damage = new Damage(damageValue, DamageType.True);
			ParentCharacter.Attack(this, character, damage);

		}
	}
}
