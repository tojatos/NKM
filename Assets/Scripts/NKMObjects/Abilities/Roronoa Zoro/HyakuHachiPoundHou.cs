using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Roronoa_Zoro
{
	public class HyakuHachiPoundHou : Ability, IClickable
	{
		private const int AbilityDamage = 18;
		private const int AbilityRange = 6;

		public HyakuHachiPoundHou() : base(AbilityType.Ultimatum, "Hyaku Hachi Pound Hou", 6)
		{
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
		}
		
		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(AbilityRange, SearchFlags.StopAtWalls | SearchFlags.StraightLine);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyEnemiesOf(Owner);

		public override string GetDescription() =>
$@"{ParentCharacter.Name} wysyła 3 fale uderzeniowe w wybranego wroga, z czego każda zadaje {AbilityDamage} obrażeń fizycznych.
Zasięg: {AbilityRange}	Czas odnowienia: {Cooldown}";

		public void Click() => Active.Prepare(this, GetTargetsInRange());

		public override void Use(Character targetCharacter)
		{
			Active.PlayAudio(Name);
			HexCell targetCell = targetCharacter.ParentCell;
			SendShockwave(targetCell);
			SendShockwave(targetCell);
			SendShockwave(targetCell);
			OnUseFinish();
		}

		private void SendShockwave(HexCell targetCell)
		{
			HexDirection direction = ParentCharacter.ParentCell.GetDirection(targetCell);
			List<HexCell> shockwaveCells = ParentCharacter.ParentCell.GetLine(direction, AbilityRange).ToList();
			foreach (HexCell c in shockwaveCells)
			{
				if (c.CharacterOnCell == null || c.CharacterOnCell.Owner == ParentCharacter.Owner) continue;

				var damage = new Damage(AbilityDamage, DamageType.Physical);
				ParentCharacter.Attack(this, c.CharacterOnCell, damage);
				break;
			}
		}
	}
}
