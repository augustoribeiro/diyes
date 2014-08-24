namespace Diyes.AppendOnlyStore.Interfaces
{
    public class DataWithVersion : IDataWithVersion
    {
        public string Identity { get; private set; }
        public int Version { get; private set; }
        public string Data { get; private set; }

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