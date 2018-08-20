namespace NKMObjects.Templates
{
    /// <summary>
    /// Abilities that implement this interface can be triggered
    /// by eg. other abilities
    /// </summary>
    public interface IRunnable
    {
        void Run();
    }
}