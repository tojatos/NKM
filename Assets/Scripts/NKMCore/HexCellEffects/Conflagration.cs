﻿using NKMCore.Hex;
using NKMCore.Templates;
using Unity.Extensions;

namespace NKMCore.HexCellEffects
{
	public class Conflagration : HexCellEffect
	{
		private readonly Character _characterThatOwnsEffect;

		public Conflagration(Game game, int cooldown, HexCell parentCell, Character characterThatOwnsEffect) : base(game, cooldown, parentCell, "Conflagration")
		{
			_characterThatOwnsEffect = characterThatOwnsEffect;
			Active.SelectDrawnCell(parentCell).AddEffectHighlight(Name);
			OnRemove += () => Active.SelectDrawnCell(parentCell).RemoveEffectHighlight(Name);
		}

		public override string GetDescription() =>
			$"{_characterThatOwnsEffect.FormattedFirstName()} może użyć podstawowego ataku na wrogu znajdującym się na tej komórce, zadając dodatkowe obrażenia."
			+ $"\nCzas do zakończenia efektu: {CurrentCooldown}";

	}
}