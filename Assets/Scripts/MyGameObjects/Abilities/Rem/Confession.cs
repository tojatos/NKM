using System;
using System.Collections.Generic;
using Helpers;
using Hex;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Rem
{
	public class Confession : Ability
	{
		private const int AbilityRange = 6;
		public Confession()
		{
			Name = "Confession";
			Cooldown = 4;
			CurrentCooldown = 0;
			Type = AbilityType.Ultimatum;
		}
		protected override void CheckIfCanBePrepared()
		{
			base.CheckIfCanBePrepared();
			List<HexCell> cellRange = GetRangeCells();
			cellRange.RemoveNonFriends();
			cellRange.RemoveAll(c=>!c.CharacterOnCell.TookActionInPhaseBefore);
			if (cellRange.Count == 0)
			{
				throw new Exception("Nie ma nikogo w zasięgu umiejętności!");
			}
		}

		public override List<HexCell> GetRangeCells()
		{
			return ParentCharacter.ParentCell.GetNeighbors(AbilityRange);
		}
		public override string GetDescription()
		{
			return string.Format(
@"{0} wyznaje miłość wybranej postaci, umożliwiając jej ponowną akcję w tej fazie.
Zasięg: {1}	Czas odnowienia: {2}",
				ParentCharacter.Name, AbilityRange, Cooldown);
		}
		protected override void Use()
		{
			List<HexCell> cellRange = GetRangeCells();
			cellRange.RemoveNonFriends();
			cellRange.RemoveAll(c => !c.CharacterOnCell.TookActionInPhaseBefore);
			Active.Prepare(this, cellRange);
		}

		public override void Use(Character character)
		{
			character.TookActionInPhaseBefore = false;
			character.HasUsedBasicAttackInPhaseBefore = false;
			character.HasUsedBasicMoveInPhaseBefore = false;
			character.HasUsedNormalAbilityInPhaseBefore = false;
			character.HasUsedUltimatumAbilityInPhaseBefore = false;
			OnUseFinish();
		}
	}
}
