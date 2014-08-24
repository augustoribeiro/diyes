namespace Diyes.AppendOnlyStore.Interfaces
{
    public interface IDataWithVersion
    {
        string Identity { get; }
        int Version { get; }
        string Data { get; }
    }
}