using ActionEffectRange.Actions.EffectRange;
using ImGuiNET;
using System.Numerics;

namespace ActionEffectRange.Drawing.Workers
{
    public interface IDrawWorker
    {
        public DrawTrigger Trigger { get; }

        public void RefreshConfig();
        public void Clear();
        public void Reset();
        public void QueueDrawing(uint sequence, EffectRangeData effectRangeData,
            Vector3 originPos, Vector3 targetPos, float rotation);
        public void CleanupOld();
        public bool HasDataToDraw();
        public void Draw(ImDrawListPtr drawList);
    }
}
