namespace NKMCore
{
    public interface ISelectable
    {
        void Select<T>(SelectableProperties<T> props);
    }
}