using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SideTopics
{
    public static class TaskIntro
    {
        public static void ShowTasks()
        {
            // TaskStatus();
            // Threads();
            // TaskArray();
        }

        private static async Task DoComplexMath()
        {
            // 1 + 2 + 3 + 4 -> (1 + 2) + (3 + 4)
            var t1 = ComplexAddAsync(1, 2);
            var t2 = ComplexAddAsync(3, 4);
            var results = await Task.WhenAll(t1, t2);
            Console.WriteLine(await ComplexAddAsync(results[0], results[1]));
        }


        private static Task<int> NullResult = Task.FromResult(0);
        private static Task<int> ComplexAddAsync(int x, int y)
        {
            if (x == 0 && y == 0)
            {
                return NullResult;
            }

            return Task.Run(() =>
            {
                Thread.Sleep(250);
                return x + y;
            });
        }

        private static void TaskArray()
        {
            var tArray = Enumerable.Range(0, 100).Select(_ => Task.Run(() =>
                Console.WriteLine($"Hi from {Thread.CurrentThread.ManagedThreadId}")))
                .ToArray();
            Task.WaitAll(tArray);
        }

        private static void Threads()
        {
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
            var t = Task.Run(() =>
            {
                Thread.Sleep(3000);
                Console.WriteLine($"Hi from {Thread.CurrentThread.ManagedThreadId}");
            });

            while (true)
            {
                Console.WriteLine("Heartbeat...");
                Thread.Sleep(250);
            }
        }

        private static void TaskStatus()
        {
            var t = new Task(() =>
            {
                Thread.Sleep(100);
                Console.WriteLine("Hi there");
            });
            Console.WriteLine(t.Status);
            t.Start();
            Thread.Sleep(100);
            Console.WriteLine(t.Status);
            Thread.Sleep(100);
            Console.WriteLine(t.Status);
        }
    }
}
