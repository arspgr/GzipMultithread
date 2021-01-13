using System.Collections.Generic;

namespace GzipMultithread.Extensions
{
    public static class QueueExtension
    {
        public static T TryDequeue<T>(this Queue<T> q) where T: class
        {
            return q.Count == 0 ? null : q.Dequeue();
        }
    }
}