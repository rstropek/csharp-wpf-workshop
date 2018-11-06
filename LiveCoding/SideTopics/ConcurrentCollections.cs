using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace SideTopics
{
    public static class ConcurrentCollections
    {
        public static void ShowConcurrentCollections()
        {
            var queue = new BlockingCollection<int>();

            var producer = Task.Run(async () =>
            {
                for (var i = 0; i < 10; i++)
                {
                    await Task.Delay(250);
                    queue.Add(i);
                }

                queue.CompleteAdding();
            });

            var consumer = Task.Run(() =>
            {
                foreach(var item in queue.GetConsumingEnumerable())
                {
                    Console.WriteLine(item);
                }
            });

            Task.WaitAll(producer, consumer);
        }
    }
}
