using ActionEffectRange.Actions;
using System;
using System.Collections.Generic;
using System.Linq;

using ExcSheets = Lumina.Excel.GeneratedSheets;

namespace ActionEffectRange.UI
{
    public static class ActionDataInterfacing
    {
        public static IEnumerable<ExcSheets.Action>? GetAllPartialMatchActionExcelRows(
            string input, bool alsoMatchId, int maxCount, 
            bool playerCombatActionOnly, Func<ExcSheets.Action, bool>? filter)
            => ActionData.ActionExcelSheet?.Where(row => row != null
                && (row.Name.RawString.Contains(input, StringComparison.CurrentCultureIgnoreCase)
                    || alsoMatchId && row.RowId.ToString().Contains(input))
                && (!playerCombatActionOnly || ActionData.IsPlayerCombatAction(row)) 
                && (filter == null || filter(row)))
            .Take(maxCount);

        public static string GetActionDescription(ExcSheets.Action row)
        {
            var classjobRow = row.ClassJob.Value;
            var classjob = classjobRow != null && classjobRow.RowId > 0
                ? $" [{classjobRow.Abbreviation}]" : string.Empty;
            var pvp = row.IsPvP ? " [PvP]" : string.Empty;
            return $"#{row.RowId} {row.Name}{classjob}{pvp}";
        }

        public static float DegToCycle(float deg) => deg / 360;
        
        public static float CycleToDeg(float cycle) => cycle * 360;

        public static float DegToRad(float deg)
            => MathF.PI * deg / 180;

        public static float RadToDeg(float rad)
            => 180 * rad / MathF.PI;
    }
}
