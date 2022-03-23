using ActionEffectRange.Actions.EffectRange;
using ActionEffectRange.Drawing.Types;
using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ActionEffectRange.Drawing
{
    public static partial class EffectRangeDrawing
    {
        private static readonly Queue<DrawData> drawData = new();

        private static uint beneficialRingColour;
        private static uint beneficialFillColour;
        private static uint harmfulRingColour;
        private static uint harmfulFillColour;


        static EffectRangeDrawing()
        {
            RefreshColour();
        }

        public static void RefreshColour()
        {
            beneficialRingColour = ImGui.ColorConvertFloat4ToU32(Plugin.Config.BeneficialColour);
            beneficialFillColour = ImGui.ColorConvertFloat4ToU32(new(
                Plugin.Config.BeneficialColour.X, Plugin.Config.BeneficialColour.Y, 
                Plugin.Config.BeneficialColour.Z, Plugin.Config.FillAlpha));
            harmfulRingColour = ImGui.ColorConvertFloat4ToU32(Plugin.Config.HarmfulColour);
            harmfulFillColour = ImGui.ColorConvertFloat4ToU32(new(
                Plugin.Config.HarmfulColour.X, Plugin.Config.HarmfulColour.Y, 
                Plugin.Config.HarmfulColour.Z, Plugin.Config.FillAlpha));
        }

        public static void Clear() => drawData.Clear();

        public static void OnTick()
        {
            if (!Plugin.Config.Enabled) return;

            if (!Plugin.IsPlayerLoaded) drawData.Clear();
            if (!drawData.Any()) return;

            ImGui.SetNextWindowSize(ImGui.GetMainViewport().Size);
            ImGuiHelpers.ForceNextWindowMainViewport();
            ImGuiHelpers.SetNextWindowPosRelativeMainViewport(Vector2.Zero);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
            ImGui.Begin("EffectRangeOverlay", 
                ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoBringToFrontOnFocus
                | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoDocking 
                | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoInputs);
            try
            {
                foreach (var data in drawData)
                {
                    if (data.ElapsedSeconds < Plugin.Config.DrawDelay) continue;
                    // Unset the AntiAliasedFill flag so compound shapes won't have ugly lines when filled
                    ImGui.GetWindowDrawList().Flags &= ~ImDrawListFlags.AntiAliasedFill;
                    data.Draw(ImGui.GetWindowDrawList());
                }
            }
            catch (System.Exception e)
            {
                PluginLog.Error($"{e}");
            }
            finally
            {
                ImGui.End();
                ImGui.PopStyleVar();
            }

            while (drawData.TryPeek(out var head) && head.ElapsedSeconds > Plugin.Config.DrawDelay + Plugin.Config.PersistSeconds) drawData.Dequeue();
        }

        public static void AddEffectRangeToDraw(EffectRangeData effectRangeData, Vector3 originPos, Vector3 targetPos, float rotation)
        {
            Plugin.LogUserDebug(
                $"AddEffectRangeToDraw => {effectRangeData}, orig={originPos}, target={targetPos}, rotation={rotation}");
            if (!Plugin.IsPlayerLoaded) 
            {
                Plugin.LogUserDebug($"---EffectRangeData not added to draw: Player is not loaded");
                return;
            }

            if (effectRangeData.IsHarmfulAction && !Plugin.Config.DrawHarmful) return;
            if (!effectRangeData.IsHarmfulAction && !Plugin.Config.DrawBeneficial) return;
            uint ringCol = effectRangeData.IsHarmfulAction ? harmfulRingColour : beneficialRingColour;
            uint fillCol = effectRangeData.IsHarmfulAction ? harmfulFillColour : beneficialFillColour;

            switch (effectRangeData)
            {
                case CircleAoEEffectRangeData circleData:
                    drawData.Enqueue(new CircleAoEDrawData(
                        targetPos, circleData.EffectRange, circleData.XAxisModifier, 
                        ringCol, fillCol));
                    break;
                case ConeAoEEffectRangeData coneData:
                    drawData.Enqueue(originPos == targetPos ?
                        new FacingDirectedConeAoEDrawData(originPos,
                            rotation + coneData.RotationOffset,
                            coneData.EffectRange, coneData.XAxisModifier,
                            coneData.CentralAngleBy2Pi, ringCol, fillCol) :
                        new TargetDirectedConeAoEDrawData(originPos, targetPos,
                            coneData.EffectRange, coneData.XAxisModifier,
                            coneData.CentralAngleBy2Pi, ringCol, fillCol));
                    break;
                case LineAoEEffectRangeData lineData:
                    drawData.Enqueue(originPos == targetPos 
                        ? new FacingDirectedLineAoEDrawData(
                            originPos, rotation + lineData.RotationOffset, lineData.EffectRange,
                            lineData.XAxisModifier, false, ringCol, fillCol) 
                        :new TargetDirectedLineAoEDrawData(
                            originPos, targetPos, lineData.EffectRange, 
                            lineData.XAxisModifier, false, ringCol, fillCol));
                    break;
                case DashAoEEffectRangeData dashData:
                    drawData.Enqueue(new DashAoEDrawData(
                        originPos, targetPos, dashData.EffectRange, 
                        dashData.XAxisModifier, ringCol, fillCol));
                    break;
                case DonutAoEEffectRangeData donutData:
                    drawData.Enqueue(new DonutAoEDrawData(
                        targetPos, donutData.EffectRange, 
                        donutData.XAxisModifier, donutData.InnerRadius, 
                        ringCol, fillCol));
                    break;
                default:
                    Plugin.LogUserDebug(
                        $"---No DrawData created for Action#{effectRangeData.ActionId}: " +
                        $"created {effectRangeData.GetType().Name} from unknown AoE type");
                    return;
            };
        }

    }
}
