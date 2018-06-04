using System;
using System.Collections.Generic;
using Helpers;
using Hex;
using MyGameObjects.MyGameObject_templates;
using System.Linq;

namespace MyGameObjects.Abilities.Shana
{
	public class KessenOgi : Ability
	{
		private const int FlameDamage = 15;
		private const int Range = 5;
		private const int FlameWidth = 1;
		private const int ShinkuKnockback = 2;
		private const int ShinpanAndDanzaiDamage = 5;
		private const int ShinpanAndDanzaiRange = 6;


		public KessenOgi()
		{
			Name = "Kessen Ōgi";
			Cooldown = 4;
			CurrentCooldown = 0;
			Type = AbilityType.Ultimatum;
		}

		public override string GetDescription()
		{
			return string.Format(
			@"{0} używa kolejno po sobie występujących umiejętności:
			Shinku:
			Odpycha wroga o {1} pola w stronę, w którą był wykonany atak.
			Hien:
			Uderza falą płomieni za {2}, fala, o szerokości {3}, ma  zasięg do lini, w której stoi odepchnięty wróg.
			Shinpan + Danzai:
			Bije {4} nieuchronnych obrażeń celowi za każdą postać (poza sobą),
			która jest w obszarze oddalonym od Shany o {5}.
			Zasięg użycia: {6} Czas odnowienia: {7}",
			ParentCharacter.Name, ShinkuKnockback, FlameDamage, FlameWidth, ShinpanAndDanzaiDamage, ShinpanAndDanzaiRange, Range, Cooldown);
		}

	protected override void CheckIfCanBePrepared()
		{
			base.CheckIfCanBePrepared();
			List<HexCell> cellRange = GetRangeCells();
			cellRange.RemoveNonEnemies();
			if (cellRange.Count == 0)
			{
				throw new Exception("Nie ma nikogo w zasięgu umiejętności!");
			}
		}

		public override List<HexCell> GetRangeCells()
		{
			return ParentCharacter.ParentCell.GetNeighbors(Range, true, false, true);
		}
		protected override void Use()
		{
			List<HexCell> cellRange = GetRangeCells();
			cellRange.RemoveNonEnemies();
			var canUseAbility = Active.Prepare(this, cellRange);
			if (canUseAbility) return;

			MessageLogger.DebugLog("Nie ma nikogo w zasięgu umiejętności!");
			OnFailedUseFinish();
		}
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
				ParentCharacter.Attack(c.CharacterOnCell, AttackType.Magical, FlameDamage);
			}
		}
		private void ShinpanAndDanzai(Character character)
		{
			int charactersNearParent = ParentCharacter.ParentCell.GetNeighbors(ShinpanAndDanzaiRange).Where(c => c.CharacterOnCell != null).Count();
			ParentCharacter.Attack(character, AttackType.True, ShinpanAndDanzaiDamage*charactersNearParent);

		}
	}
}
