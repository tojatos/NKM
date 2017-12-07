using MyGameObjects.MyGameObject_templates;

public class Stat
{
	private readonly StatType Type;// { get; set; }
	private readonly Character ParentCharacter;// { get; private set; }
	public readonly int BaseValue;// { get; private set; }

	public delegate void OnStatChange();
	public event OnStatChange StatChanged;

	private int _value;
	public int Value
	{
		get
		{
			var modifier = 0;
			ParentCharacter.Effects.ForEach(e => modifier += e.Modifier(Type));
			return _value + modifier;
		}
		set
		{
			_value = value;
			if (Type == StatType.HealthPoints && Value > BaseValue) Value = BaseValue;
			StatChanged?.Invoke();
		}
	}

	public Stat(Character parentCharacter, StatType type, int baseValue)
	{
		ParentCharacter = parentCharacter;
		Type = type;
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
	MagicalDefense
}