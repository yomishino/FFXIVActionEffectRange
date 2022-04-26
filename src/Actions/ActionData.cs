using ActionEffectRange.Actions.Data.Containers;
using ActionEffectRange.Actions.Data.Predefined;
using ActionEffectRange.Actions.Data.Template;
using ActionEffectRange.Actions.EffectRange;
using ActionEffectRange.Actions.Enums;
using ActionEffectRange.Utils;
using Lumina.Excel;
using GeneratedSheets = Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;


namespace ActionEffectRange.Actions
{
    public static class ActionData
    {
        private static readonly ActionBlacklist actionBlacklist
            = new(Plugin.Config);
        private static readonly AoETypeOverridingList aoeTypeOverridingList
            = new(Plugin.Config);
        private static readonly ConeAoeAngleOverridingList coneAoeOverridingList
            = new(Plugin.Config);

        internal static readonly ExcelSheet<GeneratedSheets.Action>? ActionExcelSheet
            = Plugin.DataManager.GetExcelSheet<GeneratedSheets.Action>();

        internal static readonly ExcelSheet<GeneratedSheets.ActionCategory>? ActionCategoryExcelSheet
            = Plugin.DataManager.GetExcelSheet<GeneratedSheets.ActionCategory>();

        public static GeneratedSheets.Action? GetActionExcelRow(uint actionId)
            => ActionExcelSheet?.GetRow(actionId);

        public static EffectRangeData? GetActionEffectRangeDataRaw(uint actionId)
        {
            var row = GetActionExcelRow(actionId);
            if (row == null)
            {
                Dalamud.Logging.PluginLog.Warning(
                    $"No Excel row found for Action#{actionId}");
                return null;
            }
            return EffectRangeData.Create(row);
        }

        // Unk46: 0 - No direct effect, e.g. nonattacking move action like AM and En Avant, pet summon&ordering actions;
        //        1 - attacking-type;
        //        2 - healing-type;
        // From observation, not sure.
        // But grounded attacking AoE (salted earth, doton, the removed ShadowFlare etc.) are all 2,
        // Celetial Opposition(pve) is 1 (probably a legacy design where it used to stun enemies?).
        public static ActionHarmfulness GetActionHarmfulness(
            GeneratedSheets.Action actionRow)
            => actionRow.Unknown46 == 1 
            ? ActionHarmfulness.Harmful : ActionHarmfulness.Beneficial;

        public static bool IsHarmfulAction(EffectRangeData data)
            => data.Harmfulness.HasFlag(ActionHarmfulness.Harmful);

        public static bool IsBeneficialAction(EffectRangeData data)
            => data.Harmfulness.HasFlag(ActionHarmfulness.Beneficial);

        public static ushort GetRecast100ms(GeneratedSheets.Action actionRow)
            => actionRow.Recast100ms;

        public static bool IsPlayerCombatAction(GeneratedSheets.Action actionRow)
            => actionRow.IsPlayerAction
            && IsCombatActionCategory((ActionCategory)actionRow.ActionCategory.Row);

        public static bool IsCombatActionCategory(ActionCategory actionCategory)
            => actionCategory is ActionCategory.Ability or ActionCategory.AR
            or ActionCategory.LB or ActionCategory.Spell or ActionCategory.WS;

        public static bool IsSpecialOrArtilleryActionCategory(ActionCategory actionCategory)
            => actionCategory is ActionCategory.Special or ActionCategory.Artillery;

        public static string GetActionCategoryName(ActionCategory actionCategory)
            => ActionCategoryExcelSheet?.GetRow((uint)actionCategory)?.Name ?? string.Empty;


        #region Overriding data managing 

        public static void ReloadCustomisedData()
        {
            actionBlacklist.Reload();
            aoeTypeOverridingList.Reload();
            coneAoeOverridingList.Reload();
        }

        public static void SaveCustomisedData(bool writeToFile = false)
        {
            actionBlacklist.Save(writeToFile);
            aoeTypeOverridingList.Save(writeToFile);
            coneAoeOverridingList.Save(writeToFile);
        }

        public static bool AddToActionBlacklist(uint actionId)
            => actionBlacklist.Add(actionId);

        public static bool RemoveFromActionBlacklist(uint actionId)
            => actionBlacklist.Remove(actionId);

        public static IEnumerable<BlacklistedActionDataItem> GetCustomisedActionBlacklistCopy()
            => actionBlacklist.CopyCustomised();

        public static bool AddToAoETypeList(
            uint actionId, byte castType, ActionHarmfulness harmfulness)
            => aoeTypeOverridingList.Add(new(actionId, castType, harmfulness));

        public static bool RemoveFromAoETypeList(uint actionId)
            => aoeTypeOverridingList.Remove(actionId);

        public static IEnumerable<AoETypeDataItem> GetCustomisedAoETypeListCopy()
            => aoeTypeOverridingList.CopyCustomised();

        public static bool AddToConeAoEAngleList(
            uint actionId, float centralAngleCycles, float rotationOffset)
            => coneAoeOverridingList.Add(new(actionId, centralAngleCycles, rotationOffset));

        public static bool RemoveFromConeAoEAngleList(uint actionId)
            => coneAoeOverridingList.Remove(actionId);

        public static IEnumerable<ConeAoEAngleDataItem> GetCustomisedConeAoEAngleListCopy()
            => coneAoeOverridingList.CopyCustomised();

        #endregion


        #region Customisation & overriding processing 

        public static bool IsActionBlacklisted(uint actionId)
            => actionBlacklist.Contains(actionId);

        public static bool CheckPetAction(EffectRangeData ownerActionData,
            out List<EffectRangeData>? petActionEffectRangeDataSet)
        {
            petActionEffectRangeDataSet = null;
            if (!PetActionMap.Dictionary.TryGetValue(
                    ownerActionData.ActionId, out var petActionIds)
                || petActionIds == null) return false;
            petActionEffectRangeDataSet = petActionIds.FlatMap(id =>
            {
                var data = GetActionEffectRangeDataRaw(id);
                if (data != null && data.EffectRange > 0)
                    return CheckEffectRangeDataOverriding(data);
                return null;
            });
            return petActionEffectRangeDataSet.Count > 0;
        }

        public static bool CheckPetLikeAction(EffectRangeData ownerActionData,
            out List<EffectRangeData>? petLikeActionEffectRangeDataSet)
        {
            petLikeActionEffectRangeDataSet = null;
            if (!PetLikeActionMap.Dictionary.TryGetValue(ownerActionData.ActionId, out var petActionIds)
                || petActionIds == null) return false;
            petLikeActionEffectRangeDataSet = petActionIds.FlatMap(id =>
            {
                var data = GetActionEffectRangeDataRaw(id);
                if (data != null && data.EffectRange > 0)
                    return CheckEffectRangeDataOverriding(data);
                return null;
            });
            return petLikeActionEffectRangeDataSet.Count > 0;
        }

        public static List<EffectRangeData> CheckEffectRangeDataOverriding(
            EffectRangeData? original)
        {
            List<EffectRangeData> updated;
            if (original == null) return new();

            updated = CheckEffectRangeDataDirectMap(original);

            updated = updated.ConvertAll(
                data => CheckAoETypeOverriding(data));
            updated = updated.ConvertAll(
                data => CheckConeAoEAngleOverriding(data));
            updated = updated.ConvertAll(
                data => CheckDonutAoERadiusOverriding(data));
            
            updated = updated.FlatMap(
                data => CheckAoEHarmfulnessOverriding(data));

            updated.FlatMap(
                data => EffectRangeCornerCases.GetUpdatedEffectDataList(data));

            return updated;
        }

        private static List<EffectRangeData> CheckEffectRangeDataDirectMap(
            EffectRangeData original)
        {
            var list = new List<EffectRangeData>();
            if (DirectMap.Dictionary.TryGetValue(original.ActionId, out var mapped))
                list.AddAllMappedNotNull(mapped,
                    id => GetActionEffectRangeDataRaw(id));
            else
                list.Add(original);
            return list;
        }
            
        private static EffectRangeData CheckAoETypeOverriding(EffectRangeData original)
        {
            if (aoeTypeOverridingList.TryGet(original.ActionId, out var data)
                && data != null)
                return EffectRangeData.Create(
                    original.ActionId, (uint)original.Category, original.IsGTAction,
                    data.Harmfulness, original.Range, original.EffectRange,
                    original.XAxisModifier, data.CastType, isOriginal: false);
            return original;
        }

        private static EffectRangeData CheckConeAoEAngleOverriding(EffectRangeData original)
        {
            if (original is not ConeAoEEffectRangeData) return original;

            if (coneAoeOverridingList.TryGet(original.ActionId, out var coneData)
                && coneData != null)
                return new ConeAoEEffectRangeData(
                    original, coneData.CentralAngleCycles, coneData.RotationOffset);

            if (ConeAoEAngleMap.DefaultAnglesByRange.TryGetValue(
                original.EffectRange, out var angle))
                return new ConeAoEEffectRangeData(original, angle);

            return new ConeAoEEffectRangeData(original, ConeAoEAngleMap.DefaultAngleCycles);
        }

        private static EffectRangeData CheckDonutAoERadiusOverriding(EffectRangeData original)
        {
            if (DonutAoERadiusMap.Predefined.TryGetValue(original.ActionId, out var radius))
                return new DonutAoEEffectRangeData(original, radius);

            return original;
        }

        private static List<EffectRangeData> CheckAoEHarmfulnessOverriding(
            EffectRangeData original)
        {
            var updated = new List<EffectRangeData>();
            if (HarmfulnessMap.Dictionary.TryGetValue(
                original.ActionId, out var harmfulness))
                updated.Add(EffectRangeData.CreateChangeHarmfulness(
                    original, harmfulness));
            else updated.Add(original);
            return updated;
        }

        #endregion
    }
}
