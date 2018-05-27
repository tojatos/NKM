using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using Hex;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.Abilities.Hecate
{
	public class AsterYo : Ability
	{
		private const int AbilityDamage = 12;
		private const int AbilityRange = 10;
		private const int AbilityRadius = 10;
		public AsterYo()
		{
			Name = "Aster Yo";
			Cooldown = 3;
			CurrentCooldown = 0;
			Type = AbilityType.Normal;
		}
		public override string GetDescription()
		{
			return string.Format(
@"{0} wystrzeliwuje promienie energii z Astera,
zadając {1} obrażeń magicznych i gromadząc Energię Życiową wszystkich trafionych celów
na wskazanym obszarze w promieniu {2}.
Zasięg: {3}	Czas odnowienia: {4}",
				ParentCharacter.Name, AbilityDamage, AbilityRadius, AbilityRange, Cooldown);
		}
		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(AbilityRange);

		protected override void Use()
		{
			List<HexCell> cellRange = GetRangeCells();
			Active.Prepare(this, cellRange, false, false);
			Active.AirSelection.Enable(AirSelection.SelectionShape.Circle, AbilityRadius);
		}
		public override void Use(List<HexCell> cells)
		{
			List<Character> characters = cells.GetCharacters();
			try
			{
				ItadakiNoKura passiveAbility = ParentCharacter.Abilities.OfType<ItadakiNoKura>().SingleOrDefault();
				if (passiveAbility == null) throw new Exception("Pasywna umiejętność nie znaleziona!");

				characters = characters.Where(c => c.Owner != ParentCharacter.Owner).ToList();
//				AnimationPlayer.Instance.AsterYo(ParentCharacter.CharacterObject.transform, characters.Select(c => c.CharacterObject.transform).ToList());
				AnimationPlayer.Add(new Animations.AsterYo(ParentCharacter.CharacterObject.transform, characters.Select(c => c.CharacterObject.transform).ToList()));
				characters.ForEach(targetCharacter =>
				{
					//if (targetCharacter.Owner == ParentCharacter.Owner) return;
					ParentCharacter.Attack(targetCharacter, AttackType.Magical, AbilityDamage);
					passiveAbility.TryCollectingEnergy(targetCharacter);
				});
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
