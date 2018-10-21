﻿using Extensions;
using Hex;
using NKMObjects.Templates;

namespace NKMObjects.HexCellEffects
{
	public class Conflagration : HexCellEffect
	{
		private readonly NKMCharacter _characterThatOwnsEffect;

		public Conflagration(int cooldown, HexCell parentCell, NKMCharacter characterThatOwnsEffect) : base(cooldown, parentCell, "Conflagration")
		{
			_characterThatOwnsEffect = characterThatOwnsEffect;
			parentCell.AddEffectHighlight(Name);
			OnRemove += () => parentCell.RemoveEffectHighlight(Name);
		}

		public override string GetDescription() =>
			$"{_characterThatOwnsEffect.FormattedFirstName()} może użyć podstawowego ataku na wrogu znajdującym się na tej komórce, zadając dodatkowe obrażenia."
			+ $"\nCzas do zakończenia efektu: {CurrentCooldown}";

	}
}
