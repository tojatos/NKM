using System;
using NKMObjects.Templates;

namespace NKMObjects.Effects
{
	public class TakenDamageModifier : Effect
	{
		public int Value;
		//Increase taken damage by value
		public TakenDamageModifier(int cooldown, int value, NKMCharacter parentCharacter, string name = null) : base(cooldown, parentCharacter, name)
		{
			Name = name ?? "Taken Damage Modifier";
			Value = value;
			Type = value <= 0 ? EffectType.Positive : EffectType.Negative;
			Character.DamageDelegate d = damage => damage.Value += (int)(damage.Value * (Value / 100f));
			parentCharacter.BeforeBeingDamaged += d;
			OnRemove += () => parentCharacter.BeforeBeingDamaged -= d;
		}
		public override string GetDescription() =>
$@"{(Value > 0 ? "Zwiększa" : "Zmniejsza")} otrzymywane obrażenia o {Math.Abs(Value)}%
Czas do zakończenia efektu: {CurrentCooldown}";
	}
}
