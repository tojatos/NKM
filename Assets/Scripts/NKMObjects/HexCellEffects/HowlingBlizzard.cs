﻿using System.Linq;
using Extensions;
using Hex;
using NKMObjects.Effects;
using NKMObjects.Templates;

namespace NKMObjects.HexCellEffects
{
	public class HowlingBlizzard : HexCellEffect
	{
		private readonly Character _characterThatOwnsEffect;
		private readonly int _speedDecrease;
		private bool _isBeingRemoved;

		public HowlingBlizzard(int cooldown, HexCell parentCell, Character characterThatOwnsEffect, int value, string name) : base(cooldown, parentCell, name)
		{
			_characterThatOwnsEffect = characterThatOwnsEffect;
			_speedDecrease = value;
			ParentCell.OnEnter += AddEffect;
			ParentCell.OnLeave += RemoveEffect;
			OnRemove += () =>
			{
				_isBeingRemoved = true;
				if (ParentCell.CharacterOnCell != null) RemoveEffect(ParentCell.CharacterOnCell);
				ParentCell.OnEnter -= AddEffect;
				ParentCell.OnLeave -= RemoveEffect;
			};
			if(ParentCell.CharacterOnCell!=null) AddEffect(ParentCell.CharacterOnCell);
		}
		private void AddEffect(Character character)
		{
			if(!character.IsEnemyFor(_characterThatOwnsEffect.Owner)) return;
			if (character.Effects.Any(e => e.Name == Name)) return;
            var effect = new StatModifier(1, -_speedDecrease, character, StatType.Speed, Name);
			effect.OnRemove += () =>
			{
				if (effect.ParentCharacter.ParentCell.Effects.ContainsType(typeof(HowlingBlizzard))&&!effect.ParentCharacter.IsLeaving&&!_isBeingRemoved)
					AddEffect(effect.ParentCharacter);
			};
            character.Effects.Add(effect);
		}
		private void RemoveEffect(Character character)
		{
//			if(!character.IsEnemyFor(_characterThatOwnsEffect.Owner)) return;
//			character.Effects.RemoveAll(e => e.Name == Name);
			character.Effects.FindAll(e => e.Name == Name).ForEach(e => e.RemoveFromParent());
		}


		public override string GetDescription() =>
			$"Wrogowie gracza {_characterThatOwnsEffect.Owner.Name}, którzy stoją na tym polu zostają spowolnieni o {_speedDecrease}."
			+ $"\nCzas do zakończenia efektu: {CurrentCooldown}";

	}
}
