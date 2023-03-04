using ActionEffectRange.Actions.Data.Containers;
using ActionEffectRange.Actions.Data.Predefined;
using ActionEffectRange.Actions.Data.Template;
using ActionEffectRange.Actions.Enums;
using Lumina.Excel;
using System.Collections.Generic;
using System.Collections.Immutable;
using GeneratedSheets = Lumina.Excel.GeneratedSheets;


namespace ActionEffectRange.Actions.Data
{
    public static class ActionData
    {
        private static readonly ActionBlacklist actionBlacklist = new(Config);
        private static readonly AoETypeOverridingList aoeTypeOverridingList
            = new(Config);
        private static readonly ConeAoeAngleOverridingList coneAoeOverridingList
            = new(Config);


        #region Data sheet related

        internal static readonly ExcelSheet<GeneratedSheets.Action>? ActionExcelSheet
            = DataManager.GetExcelSheet<GeneratedSheets.Action>();

        internal static readonly ExcelSheet<GeneratedSheets.ActionCategory>? ActionCategoryExcelSheet
            = DataManager.GetExcelSheet<GeneratedSheets.ActionCategory>();

        public static GeneratedSheets.Action? GetActionExcelRow(uint actionId)
            => ActionExcelSheet?.GetRow(actionId);

        // Unk46 seems related to actions being harmful/beneficial:
        //  0 - No direct effect, e.g. nonattacking move action like
        //      AM and En Avant, pet summon&ordering actions;
        //  1 - attacking/harmful;
        //  2 - healing/beneficial;
        // From observation, not sure.
        // But grounded attacking AoE (salted earth, doton etc.) are all 2,
        // Celetial Opposition(pve) is 1 (probably a legacy design where it used to stun enemies?)
        public static ActionHarmfulness GetActionHarmfulness(
            GeneratedSheets.Action actionRow)
            => actionRow.Unknown46 switch
            {
                1 => ActionHarmfulness.Harmful,
                2 => ActionHarmfulness.Beneficial,
                _ => ActionHarmfulness.None
            };

        // Actions used by player and actions for addictional effects / pet actions etc.
        // following player using some action
        // ** ClassJobCategory=1 ("All Classes") seem to contain mostly special actions (not combat/doh/dol job related),
        //  but also some that look like normal combat actions -- may be RP battle actions, not sure.
        // ** Meanwhile, actions from Eureka / Resistance items have ClassJobCategory>1
        // ** =0 ones are actions by enemies, Trust or quest battle NPCs, etc.
        // ** But PvP additional effect actions (e.g. #29706) may also have 0 here
        public static bool IsPlayerTriggeredAction(GeneratedSheets.Action actionRow)
            => actionRow.ClassJobCategory.Row > 0 || actionRow.IsPvP;

        public static bool IsPlayerCombatAction(GeneratedSheets.Action actionRow)
            => IsPlayerTriggeredAction(actionRow)
            && IsCombatActionCategory((ActionCategory)actionRow.ActionCategory.Row);

        public static ActionCategory GetActionCategory(uint actionId)
            => (ActionCategory)(GetActionExcelRow(actionId)?.ActionCategory.Row ?? 0);

        public static bool IsCombatActionCategory(ActionCategory actionCategory)
            => actionCategory is ActionCategory.Ability or ActionCategory.AR
            or ActionCategory.LB or ActionCategory.Spell or ActionCategory.WS;

        public static bool IsSpecialOrArtilleryActionCategory(ActionCategory actionCategory)
            => actionCategory is ActionCategory.Special or ActionCategory.Artillery;

        public static string GetActionCategoryName(ActionCategory actionCategory)
            => ActionCategoryExcelSheet?.GetRow((uint)actionCategory)?.Name ?? string.Empty;

        #endregion

        #region Customisation data managing 

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


        #region Overriding processing 

        public static bool IsActionBlacklisted(uint actionId)
            => actionBlacklist.Contains(actionId);

        // But partially based on heuristics...
        public static bool AreRelatedPlayerTriggeredActions(
            uint primaryActionId, uint secondaryActionId)
        {
            var row1 = GetActionExcelRow(primaryActionId);
            var row2 = GetActionExcelRow(secondaryActionId);
            if (row1 == null || row2 == null || !IsPlayerTriggeredAction(row1)
                || !IsPlayerTriggeredAction(row2)) return false;

            if (primaryActionId == secondaryActionId) return true;
            if (AdditionalEffectsMap.Dictionary.TryGetValue(primaryActionId,
                out var additionals) && additionals.Contains(secondaryActionId))
                return true;

            // Case: Action with same name => possibly one is an additional effect of the other
            if (row1.Name.RawString == row2.Name.RawString) return true;

            // Case: Known pet/pet-like action
            // ** Not used so not checked
            //if (PetActionMap.Dictionary.TryGetValue(primaryActionId, out var secPetIds)
            //    && secPetIds.Contains(secondaryActionId)) return true;
            //if (PetLikeActionMap.Dictionary.TryGetValue(primaryActionId, out var secPetlikeIds)
            //    && secPetlikeIds.Contains(secondaryActionId)) return true;

            // Case: other?

            return false;
        }

        public static bool ShouldNotUseCachedSeq(uint actionId)
            => NoCachedSeqActions.Set.Contains(actionId);

        //public static bool TryGetDirectlyMapped(uint actionId,
        //    out ImmutableArray<uint> mappedActionIds)
        //    => DirectMap.Dictionary.TryGetValue(actionId, out mappedActionIds);

        //public static bool TryGetMappedPetActionIds(uint actionId, 
        //    out ImmutableHashSet<uint>? petActionIds)
        //    => PetActionMap.Dictionary.TryGetValue(actionId, out petActionIds);

        //public static bool TryGetMappedPetlikeActionIds(uint actionId, 
        //    out ImmutableHashSet<uint>? petlikeActionIds)
        //    => PetActionMap.Dictionary.TryGetValue(actionId, out petlikeActionIds);

        public static bool TryGetActionWithAdditionalEffects(uint actionId,
            out ImmutableArray<uint> additionalActionIds)
            => AdditionalEffectsMap.Dictionary.TryGetValue(
                actionId, out additionalActionIds);

        public static bool TryGetModifiedAoEType(uint actionId, out AoETypeDataItem? item)
            => aoeTypeOverridingList.TryGet(actionId, out item);

        public static bool TryGetHarmfulness(uint actionId,
            out ActionHarmfulness harmfulness)
            => HarmfulnessMap.Dictionary.TryGetValue(actionId, out harmfulness);

        public static bool TryGetModifiedCone(uint actionId, out ConeAoEAngleDataItem? item)
            => coneAoeOverridingList.TryGet(actionId, out item);

        public static bool TryGetConeAoEDefaultAngle(byte effectRange, out float angle)
            => ConeAoEAngleMap.DefaultAnglesByRange.TryGetValue(effectRange, out angle);

        public static float ConeAoEDefaultAngleCycles 
            => ConeAoEAngleMap.DefaultAngleCycles;

        public static bool TryGetDonutAoERadius(uint actionId, out byte radius)
            => DonutAoERadiusMap.Predefined.TryGetValue(actionId, out radius);

        #endregion
    }
}
