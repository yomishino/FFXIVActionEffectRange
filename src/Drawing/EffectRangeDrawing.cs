using ActionEffectRange.Actions;
using ActionEffectRange.Actions.Enums;
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
            beneficialFillColour = ImGui.ColorConvertFloat4ToU32(new(Plugin.Config.BeneficialColour.X, Plugin.Config.BeneficialColour.Y, Plugin.Config.BeneficialColour.Z, Plugin.Config.FillAlpha));
            harmfulRingColour = ImGui.ColorConvertFloat4ToU32(Plugin.Config.HarmfulColour);
            harmfulFillColour = ImGui.ColorConvertFloat4ToU32(new(Plugin.Config.HarmfulColour.X, Plugin.Config.HarmfulColour.Y, Plugin.Config.HarmfulColour.Z, Plugin.Config.FillAlpha));
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
                ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoDecoration 
                | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoInputs);
            try
            {
                foreach (var data in drawData)
                {
                    if (data.ElapsedSeconds < Plugin.Config.DrawDelay) continue;
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
            Plugin.LogUserDebug($"AddEffectRangeToDraw => {effectRangeData.ActionId}, {effectRangeData.AoEType}, orig={originPos}, target={targetPos}");
            if (!Plugin.IsPlayerLoaded) 
            {
                Plugin.LogUserDebug($"---EffectRangeData not added to draw: Player is not loaded");
                return;
            }

            if (effectRangeData.IsHarmfulAction && !Plugin.Config.DrawHarmful) return;
            if (!effectRangeData.IsHarmfulAction && !Plugin.Config.DrawBeneficial) return;
            uint ringCol = effectRangeData.IsHarmfulAction ? harmfulRingColour : beneficialRingColour;
            uint fillCol = effectRangeData.IsHarmfulAction ? harmfulFillColour : beneficialFillColour;
            switch (effectRangeData.AoEType)
            {
                case ActionAoEType.Circle:
                    drawData.Enqueue(new CircleAoEDrawData(targetPos, effectRangeData.EffectRange, effectRangeData.XAxisModifier, ringCol, fillCol));
                    break;
                case ActionAoEType.Cone:
                    var angle = ActionData.GetConeAoECentralAngle(effectRangeData.ActionId, effectRangeData.EffectRange);
                    drawData.Enqueue(originPos == targetPos ?
                        new FacingDirectedConeAoEDrawData(originPos, rotation + effectRangeData.RotationOffset, effectRangeData.EffectRange, effectRangeData.XAxisModifier, angle, ringCol, fillCol) :
                        new TargetDirectedConeAoEDrawData(originPos, targetPos, effectRangeData.EffectRange, effectRangeData.XAxisModifier, angle, ringCol, fillCol));
                    break;
                case ActionAoEType.Line:
                    drawData.Enqueue(originPos == targetPos ?
                        new FacingDirectedLineAoEDrawData(originPos, rotation + effectRangeData.RotationOffset, effectRangeData.EffectRange, effectRangeData.XAxisModifier, false, ringCol, fillCol) :
                        new TargetDirectedLineAoEDrawData(originPos, targetPos, effectRangeData.EffectRange, effectRangeData.XAxisModifier, false, ringCol, fillCol));
                    break;
                case ActionAoEType.GT:
                    drawData.Enqueue(new CircleAoEDrawData(targetPos, effectRangeData.EffectRange, effectRangeData.XAxisModifier, ringCol, fillCol));
                    break;
                case ActionAoEType.DashAoE:
                    drawData.Enqueue(new DashAoEDrawData(originPos, targetPos, effectRangeData.EffectRange, effectRangeData.XAxisModifier, ringCol, fillCol));
                    break;
                case ActionAoEType.Donut:
                    drawData.Enqueue(new DonutAoEDrawData(targetPos, effectRangeData.EffectRange, effectRangeData.XAxisModifier, effectRangeData.AdditionalEffectRange, ringCol, fillCol));
                    break;
                default:
                    PluginLog.Error($"Failed to create and queue DrawData for action#{effectRangeData.ActionId}: Unknown AoE Type {effectRangeData.AoEType}");
                    return;
            }
        }

    }
}
