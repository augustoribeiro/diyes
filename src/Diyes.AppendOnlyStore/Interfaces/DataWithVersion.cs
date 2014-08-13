namespace Diyes.AppendOnlyStore.Interfaces
{
    public class DataWithVersion
    {
        public readonly string Identity;
        public readonly int Version;
        public readonly string Data;

        public DataWithVersion(string identity, int version, string data)
        {
            Identity = identity;
            Version = version;
            Data = data;
        }
    }
}