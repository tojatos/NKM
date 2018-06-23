using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Hecate
{
	public class AsterYo : Ability, IClickable
	{
		private const int AbilityDamage = 12;
		private const int AbilityRange = 10;
		private const int AbilityRadius = 7;
		public AsterYo() : base(AbilityType.Normal, "Aster Yo", 3)
		{
//			Name = "Aster Yo";
//			Cooldown = 3;
//			CurrentCooldown = 0;
//			Type = AbilityType.Normal;
		}
		public override string GetDescription() => 
$@"{ParentCharacter.Name} wystrzeliwuje promienie energii z Astera,
zadając {AbilityDamage} obrażeń magicznych i gromadząc Energię Życiową wszystkich trafionych celów
na wskazanym obszarze w promieniu {AbilityRadius}.
Zasięg: {AbilityRange}	Czas odnowienia: {Cooldown}";
		
		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(AbilityRange);

		public void ImageClick()
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
					var damage = new Damage(AbilityDamage, DamageType.Magical);
					ParentCharacter.Attack(targetCharacter, damage);
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
