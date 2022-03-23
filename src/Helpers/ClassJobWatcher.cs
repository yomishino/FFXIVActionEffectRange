namespace ActionEffectRange.Helpers
{
    public static class ClassJobWatcher
    {
        private static uint classJobId;

        public delegate void OnClassJobChangedDelegate(uint currentClassJob);
        public static event OnClassJobChangedDelegate ClassJobChanged = delegate { }; 

        private static void CheckClassJobChange(uint currentClassJobId)
        {
            if (classJobId != currentClassJobId)
            {
                classJobId = currentClassJobId;
                ClassJobChanged.Invoke(classJobId);
            }
        }

        private static void OnFrameworkUpdate(Dalamud.Game.Framework _)
        {
            if (Plugin.ClientState.LocalContentId == 0) return;
            var player = Plugin.ClientState.LocalPlayer;
            if (player == null) return;
            CheckClassJobChange(player.ClassJob.Id);
        }

        public static void Dispose()
        {
            Plugin.Framework.Update -= OnFrameworkUpdate;
        }

        static ClassJobWatcher()
        {
            Plugin.Framework.Update += OnFrameworkUpdate;
        }
    }
}
