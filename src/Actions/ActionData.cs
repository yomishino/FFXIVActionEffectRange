using ActionEffectRange.Actions.Data.Containers;
using ActionEffectRange.Actions.Data.Predefined;
using ActionEffectRange.Actions.Data.Template;
using ActionEffectRange.Actions.EffectRange;
using ActionEffectRange.Actions.Enums;
using Lumina.Excel;
using GeneratedSheets = Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;


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

        private static readonly IDictionary<uint, ActionHarmfulness>
            harmfulnessDict = HarmfulnessMap.Dictionary;

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
        public static bool IsHarmfulAction(GeneratedSheets.Action actionRow)
            => actionRow.Unknown46 == 1;

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
            uint actionId, byte castType, bool isHarmful)
            => aoeTypeOverridingList.Add(new(actionId, castType, isHarmful));

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
            out HashSet<EffectRangeData>? petActionEffectRangeDataSet)
        {
            petActionEffectRangeDataSet = null;
            if (!PetActionMap.Dictionary.TryGetValue(ownerActionData.ActionId, out var petActionIds)
                || petActionIds == null) return false;
            petActionEffectRangeDataSet = petActionIds
                .Select(id => GetActionEffectRangeDataRaw(id))
                .Where(data => data != null && data.EffectRange > 0)
                .SelectMany(data => CheckEffectRangeDataOverriding(data))
                .ToHashSet();
            return petActionEffectRangeDataSet.Any();
        }

        public static bool CheckPetLikeAction(EffectRangeData ownerActionData,
            out HashSet<EffectRangeData>? petLikeActionEffectRangeDataSet)
        {
            petLikeActionEffectRangeDataSet = null;
            if (!PetLikeActionMap.Dictionary.TryGetValue(ownerActionData.ActionId, out var petActionIds)
                || petActionIds == null) return false;
            petLikeActionEffectRangeDataSet = petActionIds
                .Select(id => GetActionEffectRangeDataRaw(id))
                .Where(data => data != null && data.EffectRange > 0)
                .SelectMany(data => CheckEffectRangeDataOverriding(data))
                .ToHashSet();
            return petLikeActionEffectRangeDataSet.Any();
        }

        public static HashSet<EffectRangeData> CheckEffectRangeDataOverriding(EffectRangeData? original)
        {
            if (original == null) return new();
            EffectRangeData updated;
            updated = CheckAoETypeOverriding(original);
            updated = CheckConeAoEAngleOverriding(updated);
            updated = CheckDonutAoERadiusOverriding(updated);
            var updatedList = CheckAoEHarmfulnessOverriding(updated);
            var updatedSet = new HashSet<EffectRangeData>();
            updatedList.ForEach(item =>
            {
                var set = EffectRangeCornerCases.GetUpdatedEffectDataSet(item);
                updatedSet.UnionWith(set);
                if (set.Count == 0) updatedSet.Add(item);
            });
            if (updatedSet.Count == 0) updatedSet.Add(updated);
            return updatedSet;
        }

        private static EffectRangeData CheckAoETypeOverriding(EffectRangeData original)
        {
            if (aoeTypeOverridingList.TryGet(original.ActionId, out var data)
                && data != null)
                return EffectRangeData.Create(
                    original.ActionId, (uint)original.Category, original.IsGTAction,
                    data.IsHarmful, original.Range, original.EffectRange,
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
            // TODO: check user overriding, return if any

            if (DonutAoERadiusMap.Predefined.TryGetValue(original.ActionId, out var radius))
                return new DonutAoEEffectRangeData(original, radius);

            return original;
        }

        private static List<EffectRangeData> CheckAoEHarmfulnessOverriding(
            EffectRangeData original)
        {
            var updated = new List<EffectRangeData>();
            if (harmfulnessDict.TryGetValue(original.ActionId, out var harmfulness))
            {
                if (harmfulness.HasFlag(ActionHarmfulness.Harmful))
                    updated.Add(EffectRangeData.CreateChangeHarmfulness(
                        original, true));
                if (harmfulness.HasFlag(ActionHarmfulness.Beneficial))
                    updated.Add(EffectRangeData.CreateChangeHarmfulness(
                        original, false));
            }
            else updated.Add(original);
            return updated;
        }

        #endregion
    }
}
