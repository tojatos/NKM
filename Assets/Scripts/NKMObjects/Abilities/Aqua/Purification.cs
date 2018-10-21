using System.Collections.Generic;
using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.Abilities.Aqua
{
	public class Purification : Ability, IClickable, IUseable
	{
		private const int AbilityRange = 5;

		public Purification() : base(AbilityType.Normal, "Purification", 4)
		{
			OnAwake += () => Validator.ToCheck.Add(Validator.AreAnyTargetsInRange);
		}
		
		public override List<HexCell> GetRangeCells() => ParentCharacter.ParentCell.GetNeighbors(AbilityRange);
		public override List<HexCell> GetTargetsInRange() => GetRangeCells().WhereOnlyFriendsOf(Owner);
		
		public override string GetDescription() => $@"{ParentCharacter.Name} rzuca oczyszczający czar na sojusznika, zdejmując z niego wszelkie negatywne efekty.
Zasięg: {AbilityRange} Czas odnowienia: {Cooldown}";


		public void Click() => Active.Prepare(this, GetTargetsInRange());
		public void Use(List<HexCell> cells) => Use(cells[0].CharacterOnCell);
		private void Use(NKMCharacter character)
		{
			character.Effects.Where(e => e.Type == EffectType.Negative).ToList().ForEach(e => e.RemoveFromParent());
			Finish();
		}

	}
}
