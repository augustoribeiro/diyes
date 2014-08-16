using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Diyes.AppendOnlyStore.Interfaces;

namespace Diyes.Sql
{
    public class SqlAppendOnlyStore : IAppendOnlyStore
    {
        private readonly string _connectionString;

        public SqlAppendOnlyStore(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Dispose()
        {
            
        }

        public void Append(string name, string data, int expectedVersion = -1)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
              
                var queryVersion = string.Format(@"select Coalesce(Max(Version),0) from Events where [Identity] = '{0}'", name);
                var version = connection.Query<int>(queryVersion).First();

                if (expectedVersion != -1 && version != expectedVersion)
                {
                    throw new AppendOnlyConcurrencyException(version,expectedVersion,name);
                }

                var insertQuery = string.Format(@"insert into Events ([Identity],Version,Data) Values('{0}',{1},'{2}')",
                    name, version + 1, data);

                try
                {
                    connection.Query(insertQuery);
                    connection.Close();  
                }
                catch (SqlException)
                {
                    connection.Close();
                    throw new AppendOnlyConcurrencyException(expectedVersion);
                }
            }
        }

        public IEnumerable<DataWithVersion> Read(string identity)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var query =
                    string.Format(
                        @"select [Identity], Data, Version from Events where [Identity] = '{0}' order by Version",
                        identity);

                var data = connection.Query<DataWithVersion>(query).GetEnumerator();

                while (data.MoveNext())
                {
                    yield return data.Current;
                }

                connection.Close();
            }
        }

        public IEnumerable<DataWithVersion> ReadAfterVersion(string identity, int version)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var query = string.Format(@"select [Identity], Data, Version from Events where [Identity] = '{0}' and Version > {1} order by Version",identity,version);

                var data = connection.Query<DataWithVersion>(query).GetEnumerator();

                while (data.MoveNext())
                {
                    yield return data.Current;
                }

                connection.Close();
            }
        }
    }
}
