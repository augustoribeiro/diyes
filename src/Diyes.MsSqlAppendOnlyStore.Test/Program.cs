using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Diyes.Sql;
using Diyes.Store.Implementation;
using Diyes.Store.Interfaces;
using Diyes.Test;

namespace sqltest
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "server=.;initial catalog=EventStore;integrated security=sspi";
            var sqlStore = new SqlAppendOnlyStore(connectionString);

            var aggregateRep = new AggregateRepository(new EventStore(sqlStore));

            var aggregateId = new Identity(Guid.NewGuid());
            var concrete = aggregateRep.Load<ConcreteAggregate>(aggregateId);

            concrete.Create();

            aggregateRep.Save(concrete);

            var task = new TaskFactory();

            var stopwatch = new Stopwatch();



            task.StartNew(() =>
            {
                for (int i = 0; i < 100000; i++)
                {
                    var concreteAggregate = aggregateRep.Load<ConcreteAggregate>(aggregateId);
                    Random random = new Random();
                    var number = random.Next(1, 42);
                    concreteAggregate.ChangeNumber(number);
                    try
                    {
                        aggregateRep.Save(concreteAggregate);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            });

            task.StartNew(() =>
            {
                for (int i = 0; i < 100000; i++)
                {
                    var concreteAggregate = aggregateRep.Load<ConcreteAggregate>(aggregateId);
                    Random random = new Random();
                    var number = random.Next(1, 42);
                    concreteAggregate.ChangeNumber(number);
                    try
                    {
                        aggregateRep.Save(concreteAggregate);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            });




            Console.ReadKey();

        }
    }
}
