using System;
using System.Collections.Generic;
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
                var transaction = connection.BeginTransaction();

                var queryVersion = string.Format(@"select Max(Version) from Events where [Identity] = '{0}'",name);
                var possibleVersion = connection.Query<int?>(queryVersion,null,transaction).FirstOrDefault();

                var version = possibleVersion.HasValue ? possibleVersion.Value : 0;

                if (expectedVersion != -1)
                {
                    if (version != expectedVersion)
                    {
                        throw new AppendOnlyConcurrencyException(version,expectedVersion,name);
                    }
                }

                var insertQuery = string.Format(@"insert into Events ([Identity],Data,Version) Values('{0}','{1}',{2})",
                    name, data, version+1);

                connection.Query(insertQuery,null,transaction);
                transaction.Commit();
                connection.Close();
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
