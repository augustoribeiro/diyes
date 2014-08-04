namespace Diyes.AppendOnlyStore.Interfaces
{
    public class DataWithVersion
    {
        public readonly string Identity;
        public readonly int Version;
        public readonly byte[] Data;

        public DataWithVersion(string identity, int version, byte[] data)
        {
            Identity = identity;
            Version = version;
            Data = data;
        }
    }
}