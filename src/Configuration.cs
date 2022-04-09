using ActionEffectRange.Actions.Data.Template;
using Dalamud.Configuration;
using Newtonsoft.Json;
using System;
using System.Numerics;

namespace ActionEffectRange
{
    [Serializable]
    public partial class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public bool Enabled = true;
        public bool EnabledPvP = true;

        public bool DrawBeneficial = true;
        public Vector4 BeneficialColour = new(.5f, .5f, 1, 1);
        public bool DrawHarmful = true;
        public Vector4 HarmfulColour = new(1, .5f, .5f, 1);

        public bool DrawOwnPets = true;
        public bool DrawGT = true;
        public bool DrawEx = false;
        
        public int LargeDrawOpt = 0;    // 0 - normal, 1 - no draw, 2 - ring only
        public int LargeThreshold = 15;
        [JsonIgnore] public static readonly string[] LargeDrawOptions 
            = new string[] { "Draw normally", "Do not draw", "Draw outline (outer ring) only" };

        public bool OuterRing = true;
        public int Thickness = 2;

        public bool Filled = true;
        public float FillAlpha = .1f;

        public int NumSegments = 100;   // smoothness

        public float DrawDelay = .4f;
        public float PersistSeconds = 1;

        public uint[] ActionBlacklist = Array.Empty<uint>();
        public AoETypeDataItem[] AoETypeList = Array.Empty<AoETypeDataItem>();
        public ConeAoEAngleDataItem[] ConeAoeAngleList = Array.Empty<ConeAoEAngleDataItem>();

        public bool LogDebug = false;
        public bool showSponsor = false;


        public void Save()
        {
            Plugin.PluginInterface.SavePluginConfig(this);
        }
    }
}
