using ActionEffectRange.Actions.Data.Predefined;
using System.Collections.Generic;
using System.Linq;

namespace ActionEffectRange.Actions.Data.Containers
{
    public class ActionBlacklist : IActionDataContainer<uint>
    {
        private readonly Configuration config;
        private readonly HashSet<uint> predefinedBlacklist;
        private readonly HashSet<uint> customisedBlacklist;

        public int PredefinedCount => predefinedBlacklist.Count;
        public int CustomisedCount => customisedBlacklist.Count;


        public ActionBlacklist(Configuration config)
        {
            this.config = config;
            predefinedBlacklist = new(RuledOutActions.PredefinedList);
            customisedBlacklist = new();
            Reload();
        }

        public void Reload()
        {
            customisedBlacklist.Clear();
            customisedBlacklist.UnionWith(config.ActionBlacklist);
        }

        public bool Contains(uint actionId)
            => customisedBlacklist.Contains(actionId)
            || predefinedBlacklist.Contains(actionId);

        public bool Add(uint actionId)
            => actionId > 0 && customisedBlacklist.Add(actionId);

        public bool Remove(uint actionId) => customisedBlacklist.Remove(actionId);

        public bool TryGet(uint actionId, out uint item)
            => customisedBlacklist.TryGetValue(actionId, out item)
            || predefinedBlacklist.TryGetValue(actionId, out item);

        public IEnumerable<uint> CopyCustomised()
            => new List<uint>(customisedBlacklist).AsEnumerable();

        public void Save(bool writeToFile = false)
        {
            config.ActionBlacklist = CopyCustomised().ToArray();
            if (writeToFile) config.Save();
        }
    }

}
