using System.Collections.Generic;

namespace ActionEffectRange.Actions.Data.Containers
{
    interface IActionDataContainer<T>
    {
        public int PredefinedCount { get; }
        public int CustomisedCount { get; }
        public int TotalCount => PredefinedCount + CustomisedCount;

        public bool Add(T item);
        public bool Remove(uint actionId);
        public bool Contains(uint actionId);
        public bool TryGet(uint actionId, out T? item);
        public IEnumerable<T> CopyCustomised();
        public void Reload();
        public void Save(bool writeToFile = false);
    }
}
