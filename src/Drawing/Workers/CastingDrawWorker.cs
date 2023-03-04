using ActionEffectRange.Actions.EffectRange;
using ActionEffectRange.Helpers;
using ImGuiNET;
using System.Collections.Generic;

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
                Config.DrawWhenCastingColour);
            castingFillColour = ImGui.ColorConvertFloat4ToU32(new(
                Config.DrawWhenCastingColour.X,
                Config.DrawWhenCastingColour.Y,
                Config.DrawWhenCastingColour.Z,
                Config.DrawWhenCastingColour.W * Config.FillAlpha));
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
            => DrawWhenCasting && drawDataQueue.Count > 0
            && ActionManagerHelper.IsCasting; 

        public void CleanupOld()
        {
            // Always clear all when at least one DrawData too old
            // as we only allow this worker to draw for one action at a time

            if (Config.DrawWhenCastingUntilCastEnd)
            {
                CheckSequence(ActionManagerHelper.CurrentSeq);
            }
            else
            {
                if (drawDataQueue.TryPeek(out var head)
                    && head.ElapsedSeconds > drawDelay + Config.PersistSeconds)
                {
                    Clear();
                }
            }
        }

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
            if (!IsPlayerLoaded || !DrawWhenCasting) return;
            LogUserDebug($"{GetType().Name}.QueueDrawing => " +
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
