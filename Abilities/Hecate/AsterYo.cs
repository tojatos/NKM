using System;
using System.Collections.Generic;
using System.Linq;
using NKMCore.Extensions;
using NKMCore.Hex;
using NKMCore.Templates;
using Unity;
using Unity.Hex;

namespace NKMCore.Abilities.Hecate
{
	public class AsterYo : Ability, IClickable, IUseableCellList
	{
		private const int Damage = 12;
		private const int Range = 10;
		private const int Radius = 6;
		
		public AsterYo(Game game) : base(game, AbilityType.Normal, "Aster Yo", 3){}
		
		public override List<HexCell> GetRangeCells() => GetNeighboursOfOwner(Range);
		
		public override string GetDescription() => 
$@"{ParentCharacter.Name} wystrzeliwuje promienie energii z Astera,
zadając {Damage} obrażeń magicznych
na wskazanym obszarze w promieniu {Radius}.

Zasięg: {Range}	Czas odnowienia: {Cooldown}";
		

		public void Click()
		{
			List<HexCell> cellRange = GetRangeCells();
			Active.Prepare(this, cellRange, false, false);
			Active.AirSelection.Enable(AirSelection.SelectionShape.Circle, Radius);
		}
		public void Use(List<HexCell> cells)
		{
			List<Character> characters = cells.GetCharacters();
			try
			{
				ItadakiNoKura passiveAbility = ParentCharacter.Abilities.OfType<ItadakiNoKura>().SingleOrDefault();

				characters = characters.Where(c => c.Owner != ParentCharacter.Owner).ToList();
				AnimationPlayer.Add(new Unity.Animations.AsterYo(HexMapDrawer.Instance.GetCharacterObject(ParentCharacter).transform, characters.Select(c => HexMapDrawer.Instance.GetCharacterObject(c).transform).ToList()));
				characters.ForEach(targetCharacter =>
				{
					var damage = new Damage(Damage, DamageType.Magical);
					ParentCharacter.Attack(this, targetCharacter, damage);
					passiveAbility?.TryCollectingEnergy(targetCharacter);
				});
				Finish();
			}
			catch (Exception e)
			{
				Console.DebugLog(e.Message);
				OnFailedUseFinish();
			}
		}

	}
}
