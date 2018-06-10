public class Damage
{
    public int Value { get; set; }
    public DamageType Type { get; }

    public Damage(int value, DamageType type)
    {
        Value = value;
        Type = type;
    }
        
}

public enum DamageType
{
    Physical,
    Magical,
    True
}