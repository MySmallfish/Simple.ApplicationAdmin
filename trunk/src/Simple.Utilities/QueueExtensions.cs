using System;
using System.Collections.Generic;

namespace Simple.Utilities
{
    public static class QueueExtensions
    {
        public static void WhileNotExhausted<T>(this Queue<T> queue, Action<T> actionOnDequeuedItem)
        {
            Requires.ArgumentNotNull(queue, "queue");
            Requires.ArgumentNotNull(actionOnDequeuedItem, "actionOnDequeuedItem");

            while (queue.Count > 0)
            {
                actionOnDequeuedItem(queue.Dequeue());
            }
        }
    }
}
