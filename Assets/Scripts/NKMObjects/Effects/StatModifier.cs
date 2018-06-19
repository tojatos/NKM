using System;
using NKMObjects.Templates;

namespace NKMObjects.Effects
{
	public class StatModifier : Effect
	{
		private readonly int _value;
		private readonly StatType _statType;
		public StatModifier(int cooldown, int value, Character parentCharacter, StatType statType, string name = null) : base(cooldown, parentCharacter, name)
		{
			_statType = statType;
			Name = name ?? "Stat Modifier";
			_value = value;
			Type = value >= 0 ? EffectType.Positive : EffectType.Negative;
		}
		public override string GetDescription()
		{
			return string.Format(
@"{0} {3} o {1}
Czas do zakończenia efektu: {2}",
				_value >= 0 ? "Zwiększa" : "Zmniejsza", Math.Abs(_value), CurrentCooldown, _statType);
		}
		public override int Modifier(StatType statType)
		{
			return statType == _statType ? _value : 0;
		}

	}
}
