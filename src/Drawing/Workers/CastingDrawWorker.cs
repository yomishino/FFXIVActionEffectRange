using ActionEffectRange.Actions.EffectRange;
using ActionEffectRange.Helpers;
using ImGuiNET;
using System.Collections.Generic;
using System.Numerics;

namespace ActionEffectRange.Drawing.Workers
{
    public class CastingDrawWorker : IDrawWorker
    {
        private readonly Queue<DrawData> drawDataQueue = new();

        private uint sequence = 0;

        private uint castingRingColour;
        private uint castingFillColour;

        private const float drawDelay = .1f;

        public DrawTrigger Trigger => DrawTrigger.Casting;

        public CastingDrawWorker()
        {
            RefreshConfig();
        }

        public void RefreshConfig()
        {
            castingRingColour = ImGui.ColorConvertFloat4ToU32(
                Plugin.Config.DrawWhenCastingColour);
            castingFillColour = ImGui.ColorConvertFloat4ToU32(new(
                Plugin.Config.DrawWhenCastingColour.X,
                Plugin.Config.DrawWhenCastingColour.Y,
                Plugin.Config.DrawWhenCastingColour.Z,
                Plugin.Config.DrawWhenCastingColour.W * Plugin.Config.FillAlpha));
        }

        public void Clear()
        {
            drawDataQueue.Clear();
        }

        public void Reset()
        {
            Clear();
            sequence = 0;
        }

        public bool HasDataToDraw()
            => Plugin.DrawWhenCasting && drawDataQueue.Count > 0
            && ActionManagerHelper.IsCasting; 

        public void CleanupOld()
        {
            // Always clear all when at least one DrawData too old
            // as we only allow this worker to draw for one action at a time

            if (Plugin.Config.DrawWhenCastingUntilCastEnd)
            {
                CheckSequence(ActionManagerHelper.CurrentSeq);
            }
            else
            {
                if (drawDataQueue.TryPeek(out var head)
                    && head.ElapsedSeconds > drawDelay + Plugin.Config.PersistSeconds)
                {
                    Clear();
                }
            }
        }

        // FIXME: won't draw if opening config UI; but normal drawing works fine
        // Problem may be because this worker clear everything when refreshing config;
        // Removed that line and test 
        public void Draw(ImDrawListPtr drawList)
        {
            if (!ActionManagerHelper.IsCasting) return;
            foreach (var data in drawDataQueue)
            {
                if (data.ElapsedSeconds < drawDelay) continue;
                data.Draw(ImGui.GetWindowDrawList());
            }
        }

        public void QueueDrawing(uint sequence, EffectRangeData effectRangeData,
            Vector3 originPos, Vector3 targetPos, float rotation)
        {
            if (!Plugin.IsPlayerLoaded || !Plugin.DrawWhenCasting) return;
            Plugin.LogUserDebug(
                $"{GetType().Name}.QueueDrawing => " +
                $"{effectRangeData}, orig={originPos}, target={targetPos}, rotation={rotation}");
            CheckSequence(sequence);
            var drawData = EffectRangeDrawing.GenerateDrawData(
                effectRangeData, castingRingColour, castingFillColour,
                originPos, targetPos, rotation);
            if (drawData != null) drawDataQueue.Enqueue(drawData);
        }

        private void CheckSequence(uint sequence)
        {
            if (sequence > this.sequence)
            {
                Clear();
                this.sequence = sequence;
            }
        }

    }
}
