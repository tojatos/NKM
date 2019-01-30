using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Shana
{
	public class KessenOgi : Ability, IClickable, IUseableCharacter
	{
		private const int FlameDamage = 15;
		private const int Range = 5;
		private const int ShinkuKnockback = 2;
		private const int ShinpanAndDanzaiDamage = 5;
		private const int ShinpanAndDanzaiRange = 6;
		private const int FlameWidth = 3;

		public KessenOgi(Game game) : base(game, AbilityType.Ultimatum, "Kessen Ōgi", 4)
		{
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
		}
			
		public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range, SearchFlags.StopAtWalls | SearchFlags.StraightLine);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereEnemiesOf(Owner);

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

		public void Use(Character character)
		{
			ParentCharacter.TryToTakeTurn();
			Shinku(character);
			Hien(character);
			ShinpanAndDanzai(character);
			Finish();
		}
		private void Shinku(Character character)
		{
			HexDirection direction = ParentCharacter.ParentCell.GetDirection(character.ParentCell);
			HexCell lastCell = character.ParentCell;
			for (int i = ShinkuKnockback; i > 0; i--) {
				HexCell cell = lastCell.GetCell(direction, 1);
				if(cell==null || !cell.IsFreeToStand) break;
				lastCell = cell;
			}
			if(lastCell!=ParentCharacter.ParentCell) character.MoveTo(lastCell);
		}
		private void Hien(Character character)
		{
			HexDirection direction = ParentCharacter.ParentCell.GetDirection(character.ParentCell);
			int range = ParentCharacter.ParentCell.GetDistance(character.ParentCell);
			List<HexCell> cells = ParentCharacter.ParentCell.GetArea(direction, range, FlameWidth);
			foreach (HexCell c in cells)
			{
				if (c.IsEmpty || !c.FirstCharacter.IsEnemyFor(Owner)) continue;
				var damage = new Damage(FlameDamage, DamageType.Magical);
				ParentCharacter.Attack(this, c.FirstCharacter, damage);
			}
		}
		private void ShinpanAndDanzai(Character character)
		{
			int charactersNearParent = GetNeighboursOfOwner(ShinpanAndDanzaiRange).SelectMany(c => c.CharactersOnCell).Count();
			int damageValue = ShinpanAndDanzaiDamage * charactersNearParent;
			var damage = new Damage(damageValue, DamageType.True);
			ParentCharacter.Attack(this, character, damage);

		}
	}
}
