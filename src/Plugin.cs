using ActionEffectRange.Actions;
using ActionEffectRange.Drawing;
using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Buddy;
using Dalamud.Game.ClientState.Objects;
using Dalamud.IoC;
using Dalamud.Plugin;
using System;

namespace ActionEffectRange
{
    public class Plugin : IDalamudPlugin
    {
        [PluginService]
        [RequiredVersion("1.0")]
        internal static DalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService]
        [RequiredVersion("1.0")]
        internal static CommandManager CommandManager { get; private set; } = null!;
        [PluginService]
        [RequiredVersion("1.0")]
        internal static DataManager DataManager { get; private set; } = null!;
        [PluginService]
        [RequiredVersion("1.0")]
        internal static SigScanner SigScanner { get; private set; } = null!;
        [PluginService]
        [RequiredVersion("1.0")]
        internal static ClientState ClientState { get; private set; } = null!;
        [PluginService]
        [RequiredVersion("1.0")]
        internal static ObjectTable ObejctTable { get; private set; } = null!;
        [PluginService]
        [RequiredVersion("1.0")]
        internal static BuddyList BuddyList { get; private set; } = null!;
        

        public string Name => "ActionEffectRange"
#if DEBUG
            + " [DEV]";
#elif TEST
            + " [TEST]";
#else
            ;
#endif

        private const string commandToggleConfig = "/actioneffectrange";

        private static bool _enabled;
        internal static bool Enabled
        {
            get => _enabled;
            set
            {
                if (value != _enabled)
                {
                    if (value) 
                    {
                        EffectRangeDrawing.Clear();
                        ActionWatcher.Enable();
                    }
                    else
                    {
                        EffectRangeDrawing.Clear();
                        ActionWatcher.Disable();
                    }
                    _enabled = value;
                }
            }
        }

        internal static Configuration Config = null!;
        internal static bool InConfig = false;

        public Plugin()
        {
            Config = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

            InitializeCommands();

            PluginInterface.UiBuilder.OpenConfigUi += OnOpenConfigUi;
            PluginInterface.UiBuilder.Draw += ConfigUi.Draw;

            PluginInterface.UiBuilder.Draw += EffectRangeDrawing.OnTick;

            ClientState.TerritoryChanged += CheckTerritory;

            RefreshConfig();
        }

        private static void InitializeCommands()
        {
            CommandManager.AddHandler(commandToggleConfig, new CommandInfo((_, _) => InConfig = !InConfig)
            {
                HelpMessage = "Toggle the Configuration Window of ActionEffectRange",
                ShowInHelp = true
            });
        }

        private static void OnOpenConfigUi()
        {
            InConfig = true;
        }

        private static void CheckTerritory(object? sender, ushort terr)
        {
            if (IsPvPZone)
                Enabled = Config.Enabled && Config.EnabledPvP;
            else
                Enabled = Config.Enabled;
        }

        internal static void RefreshConfig()
        {
            EffectRangeDrawing.RefreshColour();
            CheckTerritory(null, ClientState.TerritoryType);
            //Enabled = Config.Enabled;
        }


        public static bool IsPlayerLoaded => ClientState.LocalContentId != 0 && ClientState.LocalPlayer != null;

        public static bool IsPvPZone
            => DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.TerritoryType>()?.GetRow(ClientState.TerritoryType)?.IsPvpZone ?? false;



        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            PluginInterface.SavePluginConfig(Config);

            CommandManager.RemoveHandler(commandToggleConfig);
            
            PluginInterface.UiBuilder.Draw -= EffectRangeDrawing.OnTick;
            ClientState.TerritoryChanged -= CheckTerritory;

            PluginInterface.UiBuilder.Draw -= ConfigUi.Draw;
            PluginInterface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;

            ActionWatcher.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
