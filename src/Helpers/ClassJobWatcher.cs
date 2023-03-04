namespace ActionEffectRange.Helpers
{
    internal static class ClassJobWatcher
    {
        private static uint classJobId;

        public delegate void OnClassJobChangedDelegate(uint currentClassJob);
        public static event OnClassJobChangedDelegate ClassJobChanged = delegate { };

        public static uint CurrentClassJobId => LocalPlayer?.ClassJob.Id ?? 0;

        public static bool IsCurrentClassJobACNRelated()
            => CurrentClassJobId == 26  // ACN
            || CurrentClassJobId == 27  // SMN
            || CurrentClassJobId == 28; // SCH

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
            if (ClientState.LocalContentId == 0) return;
            if (LocalPlayer == null) return;
            CheckClassJobChange(CurrentClassJobId);
        }

        public static void Dispose()
        {
            Framework.Update -= OnFrameworkUpdate;
        }

        static ClassJobWatcher()
        {
            Framework.Update += OnFrameworkUpdate;
        }
    }
}
