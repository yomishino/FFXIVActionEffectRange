using System.Collections;
using System.Collections.Generic;

namespace ActionEffectRange.Actions
{
    public class ActionSeqRecord : IEnumerable<ActionSeqInfo>
    {
        private readonly int bound;
        private readonly Queue<ActionSeqInfo> queue;

        public ActionSeqRecord(int bound)
        {
            this.bound = bound;
            queue = new(bound);
        }


        public void Add(ActionSeqInfo info)
        {
            while (queue.Count >= bound)
                queue.Dequeue();
            queue.Enqueue(info);
        }

        public void Clear() => queue.Clear();

        IEnumerator IEnumerable.GetEnumerator()
            => queue.GetEnumerator();

        public IEnumerator<ActionSeqInfo> GetEnumerator()
            => queue.GetEnumerator();
    }

}
