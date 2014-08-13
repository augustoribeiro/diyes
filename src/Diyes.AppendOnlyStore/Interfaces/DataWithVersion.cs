namespace Diyes.AppendOnlyStore.Interfaces
{
    public class DataWithVersion
    {
        public string Identity;
        public int Version;
        public string Data;

        protected DataWithVersion()
        {
            //serialization 
        }

        public DataWithVersion(string identity, int version, string data)
        {
            Identity = identity;
            Version = version;
            Data = data;
        }
    }
}