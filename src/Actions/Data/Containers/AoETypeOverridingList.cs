using ActionEffectRange.Actions.Data.Predefined;
using ActionEffectRange.Actions.Data.Template;
using System.Collections.Generic;
using System.Linq;

namespace ActionEffectRange.Actions.Data.Containers
{
    public class AoETypeOverridingList : IActionDataContainer<AoETypeDataItem>
    {
        private readonly IDictionary<uint, AoETypeDataItem> predefinedDict;
        private readonly Dictionary<uint, AoETypeDataItem> customisedDict;
        private readonly Configuration config;

        public int PredefinedCount => predefinedDict.Count;
        public int CustomisedCount => customisedDict.Count;

        public AoETypeOverridingList(Configuration config)
        {
            this.config = config;
            predefinedDict = AoETypeOverridingMap.PredefinedSpecial;
            customisedDict = new();
            Reload();
        }

        public void Reload()
        {
            customisedDict.Clear();
            foreach (var item in config.AoETypeList)
                Add(item);
        }

        public bool Add(AoETypeDataItem item)
            => item.ActionId > 0 && customisedDict.TryAdd(item.ActionId, item);

        public bool Remove(uint actionId)
            => customisedDict.Remove(actionId);

        public bool Contains(uint actionId)
            => predefinedDict.ContainsKey(actionId)
            || customisedDict.ContainsKey(actionId);

        // Get customised data first
        public bool TryGet(uint actionId, out AoETypeDataItem? item)
            => customisedDict.TryGetValue(actionId, out item)
            || predefinedDict.TryGetValue(actionId, out item);

        public IEnumerable<AoETypeDataItem> CopyCustomised()
            => new List<AoETypeDataItem>(customisedDict.Values).AsEnumerable();

        public void Save(bool writeToFile = false)
        {
            config.AoETypeList = CopyCustomised().ToArray();
            if (writeToFile) config.Save();
        }
    }
}
