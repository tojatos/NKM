using Helpers;
using Hex;
using MyGameObjects.MyGameObject_templates;

namespace MyGameObjects.HexCellEffects
{
	public class Conflagration : HexCellEffect
	{
		private readonly Character _characterThatOwnsEffect;

		public Conflagration(int cooldown, HexCell parentCell, Character characterThatOwnsEffect) : base(cooldown, parentCell, "Conflagration")
		{
			_characterThatOwnsEffect = characterThatOwnsEffect;
		}

		public override string GetDescription() =>
			$"{_characterThatOwnsEffect.FormattedFirstName()} może użyć podstawowego ataku na wrogu znajdującym się na tej komórce, zadając dodatkowe obrażenia."
			+ $"\nCzas do zakończenia efektu: {CurrentCooldown}";

	}
}
