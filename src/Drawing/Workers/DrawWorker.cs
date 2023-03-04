using ActionEffectRange.Actions.EffectRange;
using ActionEffectRange.Actions.Enums;
using ImGuiNET;
using System.Collections.Generic;

namespace ActionEffectRange.Drawing.Workers
{
    public class DrawWorker : IDrawWorker
    {
        private readonly Queue<DrawData> drawDataQueue = new();

        private uint beneficialRingColour;
        private uint beneficialFillColour;
        private uint harmfulRingColour;
        private uint harmfulFillColour;

        public DrawTrigger Trigger => DrawTrigger.Used;

        public DrawWorker()
        {
            RefreshConfig();
        }

        public void Clear()
        {
            drawDataQueue.Clear();
        }

        public void CleanupOld()
        {
            while (drawDataQueue.TryPeek(out var head)
                && head.ElapsedSeconds > Config.DrawDelay + Config.PersistSeconds)
                drawDataQueue.Dequeue();
        }

        public void Draw(ImDrawListPtr drawList)
        {
            foreach (var data in drawDataQueue)
            {
                if (data.ElapsedSeconds < Config.DrawDelay) continue;
                    data.Draw(ImGui.GetWindowDrawList());
            }
        }

        public bool HasDataToDraw() => drawDataQueue.Count > 0;

        public void QueueDrawing(uint sequence, EffectRangeData effectRangeData, 
            Vector3 originPos, Vector3 targetPos, float rotation)
        {
            if (!IsPlayerLoaded) return;
            LogUserDebug($"{GetType().Name}.QueueDrawing => " +
                $"{effectRangeData}, orig={originPos}, target={targetPos}, rotation={rotation}");

            if (effectRangeData.Harmfulness.HasFlag(ActionHarmfulness.Harmful)
                && Config.DrawHarmful)
            {
                var drawData = EffectRangeDrawing.GenerateDrawData(
                    effectRangeData, harmfulRingColour, harmfulFillColour,
                    originPos, targetPos, rotation);
                if (drawData != null) drawDataQueue.Enqueue(drawData);
            }

            if (effectRangeData.Harmfulness.HasFlag(ActionHarmfulness.Beneficial)
                && Config.DrawBeneficial)
            {
                var drawData = EffectRangeDrawing.GenerateDrawData(
                    effectRangeData, beneficialRingColour, beneficialFillColour,
                    originPos, targetPos, rotation);
                if (drawData != null) drawDataQueue.Enqueue(drawData);
            }
        }

        public void RefreshConfig()
        {
            beneficialRingColour = ImGui.ColorConvertFloat4ToU32(
                Config.BeneficialColour);
            beneficialFillColour = ImGui.ColorConvertFloat4ToU32(new(
                Config.BeneficialColour.X, Config.BeneficialColour.Y,
                Config.BeneficialColour.Z, Config.FillAlpha));
            harmfulRingColour = ImGui.ColorConvertFloat4ToU32(
                Config.HarmfulColour);
            harmfulFillColour = ImGui.ColorConvertFloat4ToU32(new(
                Config.HarmfulColour.X, Config.HarmfulColour.Y,
                Config.HarmfulColour.Z, Config.FillAlpha));
        }

        public void Reset() => Clear();
    }
}
