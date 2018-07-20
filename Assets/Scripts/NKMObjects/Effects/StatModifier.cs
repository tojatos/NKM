using System;
using NKMObjects.Templates;

namespace NKMObjects.Effects
{
	public class StatModifier : Effect
	{
		public int Value;
		private readonly StatType _statType;
		public StatModifier(int cooldown, int value, Character parentCharacter, StatType statType, string name = null) : base(cooldown, parentCharacter, name)
		{
			_statType = statType;
			Name = name ?? "Stat Modifier";
			Value = value;
			Type = value >= 0 ? EffectType.Positive : EffectType.Negative;
		}
		public override string GetDescription()
		{
			return string.Format(
@"{0} {3} o {1}
Czas do zakończenia efektu: {2}",
				Value >= 0 ? "Zwiększa" : "Zmniejsza", Math.Abs(Value), CurrentCooldown, _statType);
		}
		public override int Modifier(StatType statType)
		{
			return statType == _statType ? Value : 0;
		}

		public override string ToString()
		{
			switch (_statType)
			{
				case StatType.BasicAttackRange:
					return Type == EffectType.Negative ? "Blind" : "BetterVision";
					break;
				case StatType.Speed:
					return Type == EffectType.Negative ? "Slow" : "SpeedUp";
				default:
					return base.ToString();
			}
		}
	}
}
