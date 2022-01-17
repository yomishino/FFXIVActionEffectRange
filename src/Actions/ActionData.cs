using ActionEffectRange.Actions.Data;
using ActionEffectRange.Actions.Enums;
using Dalamud.Logging;
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

        // Unk30: true is harmful
        // Unk46: 1 harmful, 2 beneficial, 0 is non-enmity? (such doh/dol abilities and non-attacking moving abilities )
        // These fields may not actually indicate harmful/beneficial; it just seems generally consistent with actions being harmful or not.
        // Also for #16553 (celestial opposition PvE), Unk30 is false but unk46 is 1.
        // ** For GT actions even if they do dmg (salted earth, doton), both fields are saying they are "beneficial" (unk30 == false, unk46 == 2),
        //      but i think it would be more intuitive if they are treated as "harmful" so those will be covered as corner cases.
        public static bool IsHarmfulAction(Lumina.Excel.GeneratedSheets.Action actionRow)
            => actionRow.Unknown30;
            //=> actionRow.Unknown46 == 1;


        public static bool IsRuledOutAction(uint actionId) => RuledOutActions.HashSet.Contains(actionId);

        
        public static HashSet<EffectRangeData>? CheckCornerCasesAndGetUpdatedEffectRangeData(uint actionId)
        {
            var originalData = GetActionEffectRangeDataRaw(actionId);
            if (originalData == null) return null;
#if DEBUG
            PluginLog.Debug($"---Action: id={actionId}, castType={originalData.CastType}({originalData.AoEType}), effectRange={originalData.EffectRange}, xAxisModifier={originalData.XAxisModifier}");
#endif
            var updatedData = EffectRangeCornerCases.GetUpdatedEffectDataSet(originalData);
            if (!updatedData.Any())
                updatedData.Add(originalData);
            return updatedData;
        }


        public static bool CheckPetAction(uint ownerActionId, out HashSet<EffectRangeData?>? petActionEffectRangeDataSet)
        {
            petActionEffectRangeDataSet = null;
            if (!Plugin.BuddyList.PetBuddyPresent
                || !PetActionMap.Dictionary.TryGetValue(ownerActionId, out var petActionIds) || petActionIds == null) return false;

            petActionEffectRangeDataSet = petActionIds
                .Select(id => GetActionEffectRangeDataRaw(id))
                .Where(data => data != null && data.EffectRange > 0)
                .ToHashSet();

            return petActionEffectRangeDataSet.Any();
        }
    }
}
