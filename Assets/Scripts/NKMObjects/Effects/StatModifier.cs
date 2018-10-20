﻿using System;
using Extensions;
using NKMObjects.Templates;

namespace NKMObjects.Effects
{
	public class StatModifier : Effect
	{
		private readonly StatType _statType;
		public readonly Modifier Modifier;
		public StatModifier(int cooldown, int value, Character parentCharacter, StatType statType, string name = null) : base(cooldown, parentCharacter, name)
		{
			_statType = statType;
			Name = name ?? "Stat Modifier";
			Modifier = new Modifier(value);
			Type = value >= 0 ? EffectType.Positive : EffectType.Negative;
			parentCharacter.GetStat(statType).Modifiers.Add(Modifier);
			OnRemove += () => parentCharacter.GetStat(statType).Modifiers.Remove(Modifier);
		}

		public override string GetDescription()
		{
			return string.Format(
@"{0} {3} o {1}
Czas do zakończenia efektu: {2}",
				Modifier.Value >= 0 ? "Zwiększa" : "Zmniejsza", Math.Abs(Modifier.Value), CurrentCooldown, _statType);
		}
//		public override int Modifier(StatType statType)
//		{
//			return statType == _statType ? Value : 0;
//		}

		public override string ToString()
		{
			switch (_statType)
			{
				case StatType.BasicAttackRange:
					return Type == EffectType.Negative ? "Blind" : "BetterVision";
				case StatType.Speed:
					return Type == EffectType.Negative ? "Slow" : "SpeedUp";
				default:
					return base.ToString();
			}
		}
	}
}
