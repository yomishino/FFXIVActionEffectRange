using System.Collections.Generic;

namespace ActionEffectRange.Actions.Data.Containers
{
    interface IActionDataContainer<IDataItem>
    {
        public int PredefinedCount { get; }
        public int CustomisedCount { get; }
        public int TotalCount => PredefinedCount + CustomisedCount;

        public bool Add(IDataItem item);
        public bool Remove(uint actionId);
        public bool Contains(uint actionId);
        public bool TryGet(uint actionId, out IDataItem? item);
        public IEnumerable<IDataItem> CopyCustomised();
        public void Reload();
        public void Save(bool writeToFile = false);
    }
}
