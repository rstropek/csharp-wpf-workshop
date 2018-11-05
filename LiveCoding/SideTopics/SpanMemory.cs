using System;
using System.Collections.Generic;
using System.Text;

namespace SideTopics
{
    public static class SpanMemory
    {
        public static void ShowSpanMemory()
        {
            var byteList = new List<byte>();
            Span<byte> byteArray = new byte[1024];
            Span<byte> byteStackArray = stackalloc byte[128];

            Span<byte> subArray = byteArray.Slice(5, 10);
            Console.WriteLine(subArray[20]);
        }
    }
}
