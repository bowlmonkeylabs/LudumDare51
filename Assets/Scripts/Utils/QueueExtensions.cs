using System.Collections;
using System.Collections.Generic;

namespace BML.Scripts.Utils
{
    public static class QueueExtensions
    {
        public static T SoftDequeue<T>(this Queue<T> queue, int preserveCount)
        {
            if (queue.Count <= preserveCount) return queue.Peek();
            else return queue.Dequeue();
        }
    }
}