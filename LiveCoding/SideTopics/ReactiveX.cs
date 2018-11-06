using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace SideTopics
{
    public static class ReactiveX
    {
        public static void ShowRx()
        {
            var subject = new Subject<int>();

            var producer = Task.Run(async () =>
            {
                for (var i = 0; i < 10; i++)
                {
                    await Task.Delay(250);
                    subject.OnNext(i);
                }

                subject.OnCompleted();
            });

            subject.Subscribe(n => Console.WriteLine(n));

            Task.WaitAll(producer);
        }
    }
}
