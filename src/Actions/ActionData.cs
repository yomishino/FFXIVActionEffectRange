using ActionEffectRange.Actions.Data;
using ActionEffectRange.Actions.Enums;
using System.Collections.Generic;
using System.Linq;

namespace ActionEffectRange.Actions
{
    public static class ActionData
    {   
        // TODO: blacklist
        //private static readonly HashSet<uint> blacklist;

        public static readonly Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.Action>? ActionExcelSheet 
            = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>();

        public static Lumina.Excel.GeneratedSheets.Action? GetActionExcelRow(uint actionId)
            => ActionExcelSheet?.GetRow(actionId);

        public static EffectRangeData? GetActionEffectRangeDataRaw(uint actionId)
        {
            var row = GetActionExcelRow(actionId);
            return row != null ? new(row) : null;
        }

        public static ActionAoEType GetActionAoEType(byte CastType)
            => CastType switch
            {
                2 => ActionAoEType.Circle,
                3 => ActionAoEType.Cone,
                4 => ActionAoEType.Line,
                7 => ActionAoEType.GT,
                8 => ActionAoEType.DashAoE,
                10 => ActionAoEType.Donut,
                _ => ActionAoEType.None
            };

        // Unk46: 0 - No direct effect, e.g. nonattacking move action like AM and En Avant, pet summon&ordering actions;
        //        1 - attacking-type;
        //        2 - healing-type;
        // From observation, not sure.
        // But grounded attacking AoE (salted earth, doton, the removed ShadowFlare etc.) are all 2,
        // Celetial Opposition(pve) is 1 (probably a legacy design where it used to stun enemies?).
        public static bool IsHarmfulAction(Lumina.Excel.GeneratedSheets.Action actionRow)
            => actionRow.Unknown46 == 1;

        public static ushort GetRecast100ms(Lumina.Excel.GeneratedSheets.Action actionRow)
            => actionRow.Recast100ms;


        public static bool IsRuledOutAction(uint actionId) => RuledOutActions.HashSet.Contains(actionId);

        
        public static HashSet<EffectRangeData> CheckCornerCasesAndGetUpdatedEffectRangeData(EffectRangeData originalData)
        {
            var updatedData = EffectRangeCornerCases.GetUpdatedEffectDataSet(originalData);
            if (!updatedData.Any())
                updatedData.Add(originalData);
            return updatedData;
        }

        public static EffectRangeData UpdateConeAoECentralAngle(EffectRangeData originalData)
            => originalData.AoEType == ActionAoEType.Cone && ConeAoEAngleMap.Dictionary.TryGetValue(originalData.ActionId, out float angle)
            ? new(originalData, centralAngleBy2pi: angle) : originalData;

        public static bool CheckPetAction(EffectRangeData ownerActionData, out HashSet<EffectRangeData?>? petActionEffectRangeDataSet)
        {
            petActionEffectRangeDataSet = null;
            if (!PetActionMap.Dictionary.TryGetValue(ownerActionData.ActionId, out var petActionIds) || petActionIds == null) return false;
            petActionEffectRangeDataSet = petActionIds
                .Select(id => GetActionEffectRangeDataRaw(id))
                .Where(data => data != null && data.EffectRange > 0)
                .ToHashSet();
            return petActionEffectRangeDataSet.Any();
        }

        public static bool CheckPetLikeAction(EffectRangeData ownerActionData, out HashSet<EffectRangeData?>? petLikeActionEffectRangeDataSet)
        {
            petLikeActionEffectRangeDataSet = null;
            if (!PetLikeActionMap.Dictionary.TryGetValue(ownerActionData.ActionId, out var petActionIds) || petActionIds == null) return false;
            petLikeActionEffectRangeDataSet = petActionIds
                .Select(id => GetActionEffectRangeDataRaw(id))
                .Where(data => data != null && data.EffectRange > 0)
                .ToHashSet();
            return petLikeActionEffectRangeDataSet.Any();
        }


        public static float GetConeAoECentralAngle(uint actionId)
            => ConeAoEAngleMap.Dictionary.TryGetValue(actionId, out float angle) ? angle : ConeAoEAngleMap.DefaultAngleBy2pi;
    }
}
