using ActionEffectRange.Actions.EffectRange;
using ActionEffectRange.Drawing.Types;
using ActionEffectRange.Drawing.Workers;
using Dalamud.Interface;
using Dalamud.Logging;
using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;

namespace ActionEffectRange.Drawing
{
    public static class EffectRangeDrawing
    {
        private static readonly List<IDrawWorker> workers = new();
        

        static EffectRangeDrawing()
        {
            workers.Add(new DrawWorker());
            workers.Add(new CastingDrawWorker());
        }

        public static void RefreshConfig()
            => workers.ForEach(worker => worker.RefreshConfig());

        private static void Clear()
            => workers.ForEach(worker => worker.Clear());

        public static void Reset()
            => workers.ForEach(worker => worker.Reset());

        public static void OnTick()
        {
            if (!Plugin.Config.Enabled) return;

            if (!Plugin.IsPlayerLoaded)
            {
                Clear();
                return;
            }

            workers.ForEach(worker => worker.CleanupOld());

            if (!HasDataToDraw()) return;

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
                // Unset the AntiAliasedFill flag so
                // compound shapes won't have ugly lines when filled
                ImGui.GetWindowDrawList().Flags &= ~ImDrawListFlags.AntiAliasedFill;

                workers.ForEach(worker => worker.Draw(ImGui.GetWindowDrawList()));
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
        }

        public static void AddEffectRangeToDraw(uint sequence,
            DrawTrigger trigger, EffectRangeData effectRangeData,
            Vector3 originPos, Vector3 targetPos, float rotation)
            => workers.ForEach(worker => 
            { 
                if (worker.Trigger == trigger) 
                    worker.QueueDrawing(sequence, effectRangeData, 
                        originPos, targetPos, rotation);
            });
        
        public static DrawData? GenerateDrawData(
            EffectRangeData effectRangeData, uint ringCol, uint fillCol,
            Vector3 originPos, Vector3 targetPos, float rotation)
        {
            switch (effectRangeData)
            {
                case CircleAoEEffectRangeData circleData:
                    return new CircleAoEDrawData(
                        targetPos, circleData.EffectRange, circleData.XAxisModifier,
                        ringCol, fillCol);
                case ConeAoEEffectRangeData coneData:
                    return originPos == targetPos 
                        ? new FacingDirectedConeAoEDrawData(originPos,
                            rotation + coneData.RotationOffset,
                            coneData.EffectRange, coneData.XAxisModifier,
                            coneData.CentralAngleCycles, ringCol, fillCol) 
                        : new TargetDirectedConeAoEDrawData(originPos, targetPos,
                            coneData.EffectRange, coneData.XAxisModifier,
                            coneData.CentralAngleCycles, coneData.RotationOffset,
                            ringCol, fillCol);
                case LineAoEEffectRangeData lineData:
                    return originPos == targetPos
                        ? new FacingDirectedLineAoEDrawData(
                            originPos, rotation, lineData.EffectRange,
                            lineData.XAxisModifier, lineData.RotationOffset, 
                            ringCol, fillCol)
                        : new TargetDirectedLineAoEDrawData(
                            originPos, targetPos, lineData.EffectRange,
                            lineData.XAxisModifier, false, 
                            lineData.RotationOffset, ringCol, fillCol);
                case DashAoEEffectRangeData dashData:
                    return new DashAoEDrawData(
                        originPos, targetPos, dashData.EffectRange,
                        dashData.XAxisModifier, ringCol, fillCol);
                case DonutAoEEffectRangeData donutData:
                    return new DonutAoEDrawData(
                        targetPos, donutData.EffectRange,
                        donutData.XAxisModifier, donutData.InnerRadius,
                        ringCol, fillCol);
                case CrossAoEEffectRangeData crossData:
                    return new CrossAoEDrawData(
                        originPos, targetPos, crossData.EffectRange,
                        crossData.XAxisModifier, crossData.RotationOffset, 
                        ringCol, fillCol);
                default:
                    Plugin.LogUserDebug(
                        $"---No DrawData created for Action#{effectRangeData.ActionId}: " +
                        $"created {effectRangeData.GetType().Name} from unknown AoE type");
                    return null;
            };
        }

        private static bool HasDataToDraw()
            => workers.Exists(worker => worker.HasDataToDraw());
    }
}
