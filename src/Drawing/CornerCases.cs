using ActionEffectRange.Actions;
using ActionEffectRange.Drawing.Types;
using System;
using System.Numerics;

namespace ActionEffectRange.Drawing
{
    public static partial class EffectRangeDrawing
    {
        // Return true if catched corner cases and finished process so that the normal drawing method should not process any further
        // These are actions that require special treatment in drawing and not catched by ActionValidator
        private static bool CheckCornerCases(EffectRangeData effectRangeData, Vector3 targetPos)
        {
            switch (effectRangeData.ActionId)
            {
                case 11430:     // glass dance (BLU)
                    // Add the two cones on side here; front cone processed as normal
                    // TODO: glass dance - not accurate tho
                    if (Plugin.Config.DrawHarmful)
                    {
                        var selfPos = Plugin.ClientState.LocalPlayer!.Position;
                        var selfRot = Plugin.ClientState.LocalPlayer!.Rotation;
                        drawData.Enqueue(new FacingDirectedConeAoEDrawData(selfPos, selfRot,
                            effectRangeData.EffectRange, effectRangeData.XAxisModifier, harmfulRingColour, harmfulFillColour, MathF.PI));
                        drawData.Enqueue(new FacingDirectedConeAoEDrawData(selfPos, selfRot - MathF.PI / 2,
                            effectRangeData.EffectRange, effectRangeData.XAxisModifier, harmfulRingColour, harmfulFillColour, MathF.PI / 4));
                        drawData.Enqueue(new FacingDirectedConeAoEDrawData(selfPos, selfRot + MathF.PI / 2,
                            effectRangeData.EffectRange, effectRangeData.XAxisModifier, harmfulRingColour, harmfulFillColour, MathF.PI / 4));
                    }
                    return true;
                default:
                    return false;
            }
        }

    }
}
