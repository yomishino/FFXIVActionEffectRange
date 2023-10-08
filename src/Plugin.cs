global using static ActionEffectRange.Game;
global using static ActionEffectRange.Plugin;
global using ActionEffectRange.Utils;
global using System;
global using System.Numerics;

using ActionEffectRange.Actions;
using ActionEffectRange.Actions.Data;
using ActionEffectRange.Drawing;
using ActionEffectRange.Helpers;
using ActionEffectRange.UI;
using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace ActionEffectRange
{
    public class Plugin : IDalamudPlugin
    {
        [PluginService]
        internal static DalamudPluginInterface PluginInterface { get; private set; } = null!;
        [PluginService]
        internal static ICommandManager CommandManager { get; private set; } = null!;
        [PluginService]
        internal static IDataManager DataManager { get; private set; } = null!;
        [PluginService]
        internal static ISigScanner SigScanner { get; private set; } = null!;
        [PluginService]
        internal static IGameInteropProvider InteropProvider { get; private set; } = null!;
        [PluginService]
        internal static IFramework Framework { get; private set; } = null!;
        [PluginService]
        internal static IClientState ClientState { get; private set; } = null!;
        [PluginService]
        internal static IObjectTable ObejctTable { get; private set; } = null!;
        [PluginService]
        internal static IBuddyList BuddyList { get; private set; } = null!;
        [PluginService]
        internal static IPluginLog PluginLog { get; private set; } = null!;
        

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
                        EffectRangeDrawing.Reset();
                        ActionWatcher.Enable();
                    }
                    else
                    {
                        EffectRangeDrawing.Reset();
                        ActionWatcher.Disable();
                    }
                    _enabled = value;
                }
            }
        }
        internal static bool DrawWhenCasting;
        
        internal static Configuration Config = null!;
        internal static bool InConfig = false;

        public Plugin()
        {
            Config = PluginInterface.GetPluginConfig() as Configuration 
                ?? new Configuration();

            InitializeCommands();

            PluginInterface.UiBuilder.OpenConfigUi += OnOpenConfigUi;
            PluginInterface.UiBuilder.Draw += ConfigUi.Draw;

            PluginInterface.UiBuilder.Draw += EffectRangeDrawing.OnTick;

            ClientState.Logout += OnLogOut;
            ClientState.TerritoryChanged += CheckTerritory;

            RefreshConfig(true);
        }

        private static void InitializeCommands()
        {
            CommandManager.AddHandler(commandToggleConfig, 
                new CommandInfo((_, _) => InConfig = !InConfig)
            {
                HelpMessage = "Toggle the Configuration Window of ActionEffectRange",
                ShowInHelp = true
            });
        }

        private static void OnOpenConfigUi()
        {
            InConfig = true;
        }

        private static void CheckTerritory(ushort terr)
        {
            if (IsPvPZone)
            {
                Enabled = Config.Enabled && Config.EnabledPvP;
                DrawWhenCasting = false;
            }
            else
            {
                Enabled = Config.Enabled;
                DrawWhenCasting = Config.DrawWhenCasting;
            }
        }

        private static void OnLogOut()
        {
            EffectRangeDrawing.Reset();
        }

        internal static void RefreshConfig(bool reloadSavedList = false)
        {
            EffectRangeDrawing.RefreshConfig();
            CheckTerritory(ClientState.TerritoryType); 

            if (reloadSavedList)
                ActionData.ReloadCustomisedData();
        }

        public static void LogUserDebug(string msg)
        {
            if (Config.LogDebug) PluginLog.Debug(msg);
        }


        #region IDisposable Support
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            PluginInterface.SavePluginConfig(Config);

            CommandManager.RemoveHandler(commandToggleConfig);
            
            PluginInterface.UiBuilder.Draw -= EffectRangeDrawing.OnTick;

            ClientState.Logout -= OnLogOut;
            ClientState.TerritoryChanged -= CheckTerritory;

            PluginInterface.UiBuilder.Draw -= ConfigUi.Draw;
            PluginInterface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;

            ActionWatcher.Dispose();
            ClassJobWatcher.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
