using NKMObjects.Templates;

public class Stat
{
	private readonly StatType _type;
	private readonly Character _parentCharacter;
	public readonly int BaseValue;

	public delegate void OnStatChange();
	public event OnStatChange StatChanged;

	public int Bonus => Value - BaseValue;
	public int RealValue { get; private set; }
	public int Value
	{
		get
		{
			int modifier = 0;
			_parentCharacter.Effects.ForEach(e => modifier += e.Modifier(_type));
			return RealValue + modifier;
		}
		set
		{
			RealValue = value;
			if (_type == StatType.HealthPoints && Value > BaseValue) Value = BaseValue;
			StatChanged?.Invoke();
		}
	}



	public Stat(Character parentCharacter, StatType type, int baseValue)
	{
		_parentCharacter = parentCharacter;
		_type = type;
		BaseValue = baseValue;
		Value = BaseValue;
	}

	public override string ToString()
	{
		return Value.ToString();
	}
}
public enum StatType
{
	HealthPoints,
	AttackPoints,
	BasicAttackRange,
	Speed,
	PhysicalDefense,
	MagicalDefense,
	Shield,
}