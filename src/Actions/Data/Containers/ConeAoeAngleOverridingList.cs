using ActionEffectRange.Actions.Data.Predefined;
using ActionEffectRange.Actions.Data.Template;
using System.Collections.Generic;
using System.Linq;

namespace ActionEffectRange.Actions.Data.Containers
{
    public class ConeAoeAngleOverridingList : IActionDataContainer<ConeAoEAngleDataItem>
    {
        private readonly IDictionary<uint, ConeAoEAngleDataItem> predefinedDict;
        private readonly Dictionary<uint, ConeAoEAngleDataItem> customisedDict;
        private readonly Configuration config;

        public int PredefinedCount => predefinedDict.Count;
        public int CustomisedCount => customisedDict.Count;


        public ConeAoeAngleOverridingList(Configuration config)
        {
            this.config = config;
            predefinedDict = ConeAoEAngleMap.PredefinedActionMap;
            customisedDict = new();
            Reload();
        }

        public void Reload()
        {
            customisedDict.Clear();
            foreach (var item in config.ConeAoeAngleList)
                Add(item);
        }

        public bool Add(ConeAoEAngleDataItem item)
            => item.ActionId > 0 && customisedDict.TryAdd(item.ActionId, item);

        public bool Remove(uint actionId)
            => customisedDict.Remove(actionId);

        public bool Contains(uint actionId)
            => predefinedDict.ContainsKey(actionId) 
            || customisedDict.ContainsKey(actionId);

        // Get customised data first
        public bool TryGet(uint actionId, out ConeAoEAngleDataItem? item)
            => customisedDict.TryGetValue(actionId, out item)
            || predefinedDict.TryGetValue(actionId, out item);

        public IEnumerable<ConeAoEAngleDataItem> CopyCustomised()
            => new List<ConeAoEAngleDataItem>(customisedDict.Values).AsEnumerable();

        public void Save(bool writeToFile = false)
        {
            config.ConeAoeAngleList = CopyCustomised().ToArray();
            if (writeToFile) config.Save();
        }
    }
}
