using ActionEffectRange.Actions.Data;
using ActionEffectRange.Actions.Enums;
using System.Collections.Generic;

namespace ActionEffectRange.Actions
{
    public static class ActionData
    {
        // TODO: blacklist
        //private static readonly HashSet<uint> blacklist;  
        private static readonly Dictionary<uint, HashSet<uint>> petActionMap;

        public static readonly Lumina.Excel.ExcelSheet<Lumina.Excel.GeneratedSheets.Action>? ActionExcelSheet 
            = Plugin.DataManager.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>();

        public static Lumina.Excel.GeneratedSheets.Action? GetActionExcelRow(uint actionId)
            => ActionExcelSheet?.GetRow(actionId);

        public static EffectRangeData? GetActionEffectRangeData(uint actionId)
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
                10 => ActionAoEType.Donut,
                _ => ActionAoEType.None
            };

        // Unk46: 1 harmful, 2 beneficial, 0 is non-enmity? (such doh/dol abilities and non-attacking moving abilities )
        public static bool IsHarmfulAction(Lumina.Excel.GeneratedSheets.Action actionRow)
            => actionRow.Unknown46 == 1;

        public static bool TryGetMappedPetAction(uint ownerActionId, out HashSet<uint>? petActionIds)
            => petActionMap.TryGetValue(ownerActionId, out petActionIds);



        static ActionData()
        {
            petActionMap = PetActionMap.AsDictionary();
        }
    }
}
