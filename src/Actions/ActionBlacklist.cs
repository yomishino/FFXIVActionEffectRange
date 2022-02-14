using System.Collections.Generic;
using System.Linq;

namespace ActionEffectRange.Actions
{
    public class ActionBlacklist
    {
        private readonly HashSet<uint> blacklist = new();
        private readonly Configuration config;

        public int Count => blacklist.Count;

        public ActionBlacklist(Configuration config) => this.config = config;

        public void Reload()
        {
            blacklist.Clear();
            blacklist.UnionWith(config.ActionBlacklist);
        }

        public void Save()
        {
            config.ActionBlacklist = ToArray();
        }

        public uint[] ToArray() => blacklist.ToArray();

        public bool Contains(uint actionId)
            => blacklist.Contains(actionId);


        public bool Add(uint actionId)
            => actionId > 0 && blacklist.Add(actionId);

        public bool Remove(uint actionId) => blacklist.Remove(actionId);

    }
}
