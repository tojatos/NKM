namespace NKMObjects.Templates
{
    /// <summary>
    /// Abilities that implement this interface show whether they are enabled or not
    /// </summary>
    public interface IEnableable
    {
       bool IsEnabled { get; } 
    }
}