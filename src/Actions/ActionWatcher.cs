using ActionEffectRange.Drawing;
using Dalamud.Hooking;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

namespace ActionEffectRange.Actions
{
    public static class ActionWatcher
    {
        private static readonly IntPtr actionMgrPtr;

        // Send what is executed; won't be called if queued but not yet executed or failed to execute (e.g. cast cancelled)
        // Information here also more accurate than from UseAction; handles combo/proc and target issues esp. with plugins like FFXIVCombo / ReAction 
        // Not called for GT actions tho
        private delegate void SendActionDelegate(long targetObjectId, byte actionType, uint actionId, ushort sequence, long a5, long a6, long a7, long a8, long a9);
        private static readonly Hook<SendActionDelegate>? SendActionHook;
        private static void SendActionDetour(long targetObjectId, byte actionType, uint actionId, ushort sequence, long a5, long a6, long a7, long a8, long a9)
        {
            SendActionHook!.Original(targetObjectId, actionType, actionId, sequence, a5, a6, a7, a8, a9);
#if DEBUG
            PluginLog.Debug($"SendAction: targetId={targetObjectId:X}, actionType={actionType}, actionId={actionId}, seq={sequence}, a5={a5:X}, a6={a6:X}, a7={a7:X}, a8={a8:X}, a9={a9:X}");
            PluginLog.Debug($"---AcMgr: currentSeq{CurrentSeq}, lastRecSeq={LastRecievedSeq}");
#endif
            if (!Plugin.IsPlayerLoaded || !ShouldProcessAction(actionType, actionId)) return;

            var originalData = ActionData.GetActionEffectRangeDataRaw(actionId);
            if (originalData == null)
            {
                PluginLog.Error($"SendAction: No excel row found for action of id {actionId}");
                return;
            }
#if DEBUG
            PluginLog.Debug($"---Action: id={actionId}, castType={originalData.CastType}({originalData.AoEType}), effectRange={originalData.EffectRange}, xAxisModifier={originalData.XAxisModifier}");
#endif

            bool replacedAction = false;

            // Check pet/pet-like actions; sequence is always 0 in ReceiveActionEffect for these actions 
            if (Plugin.BuddyList.PetBuddyPresent)
            {
                var pet = Plugin.BuddyList.PetBuddy?.GameObject;
                if (pet != null)
                {
#if DEBUG
                    PluginLog.Debug($"---check pet/pet-like action: pet objId={pet.ObjectId:X}, pos={pet.Position}");
#endif

                    if (ActionData.CheckPetLikeAction(originalData, out var petLikeActionEffectRangeDataSet) && petLikeActionEffectRangeDataSet != null)
                    {
                        replacedAction = true;
                        foreach (var data in petLikeActionEffectRangeDataSet)
                        {
                            if (data == null) continue;
                            if (!ShouldDrawForEffectRange(data.CastType, data.EffectRange)) continue;
                            if (data.IsHarmfulAction && !Plugin.Config.DrawHarmful || !data.IsHarmfulAction && !Plugin.Config.DrawBeneficial) continue;
                            recordedPetLikeActionEffectToWait[data.ActionId] = new(sequence);
                            if (data.Range == 0)
                                recordedPetLikeActionEffectToWait[data.ActionId].Add(new(data, pet.Position, pet.Position, pet.Rotation, true));
                            else
                            {
                                var target = Plugin.ObejctTable.SearchById((uint)targetObjectId);
                                if (target != null) recordedPetLikeActionEffectToWait[data.ActionId].Add(new(data, pet.Position, target.Position, pet.Rotation, true));
                                else PluginLog.Error($"SendAction: Cannot find valid target of id {targetObjectId:X} for action {actionId}");
                            }
    #if DEBUG
                            PluginLog.Debug($"---Found possible pet-like action: actionId={data.ActionId}");
    #endif
                        }
                    }

                    if (Plugin.Config.DrawOwnPets && ActionData.CheckPetAction(originalData, out var petActionEffectRangeDataSet)
                        && petActionEffectRangeDataSet != null)
                    {
                        replacedAction = true;
                        foreach (var data in petActionEffectRangeDataSet)
                        {
                            if (data == null) continue;
                            if (!ShouldDrawForEffectRange(data.CastType, data.EffectRange)) continue;
                            if (data.IsHarmfulAction && !Plugin.Config.DrawHarmful || !data.IsHarmfulAction && !Plugin.Config.DrawBeneficial) continue;
                            recordedPetActionEffectToWait[data.ActionId] = new(sequence);
                            if (data.Range == 0)
                                recordedPetActionEffectToWait[data.ActionId].Add(new(data, pet.Position, pet.Position, pet.Rotation, true));
                            else
                            {
                                var target = Plugin.ObejctTable.SearchById((uint)targetObjectId);
                                if (target != null) recordedPetActionEffectToWait[data.ActionId].Add(new(data, pet.Position, target.Position, pet.Rotation, true));
                                else PluginLog.Error($"SendAction: Cannot find valid target of id {targetObjectId:X} for action {actionId}");
                            }
#if DEBUG
                            PluginLog.Debug($"---Found possible pet action: actionId={data.ActionId}");
#endif
                        }
                    }
                }
            }

            if (replacedAction) return;

            var updatedEffectRangeDataSet = ActionData.CheckCornerCasesAndGetUpdatedEffectRangeData(originalData);

            recordedActionSequence[sequence] = new(sequence);

            foreach (var data in updatedEffectRangeDataSet)
            {
                if (!ShouldDrawForEffectRange(data.CastType, data.EffectRange)) continue;
                if (data.IsHarmfulAction && !Plugin.Config.DrawHarmful || !data.IsHarmfulAction && !Plugin.Config.DrawBeneficial) continue;

                if (data.Range == 0)
                    recordedActionSequence[sequence].Add(new(data, Plugin.ClientState.LocalPlayer!.Position, Plugin.ClientState.LocalPlayer!.Position, Plugin.ClientState.LocalPlayer!.Rotation, false));
                else
                {
                    var target = Plugin.ObejctTable.SearchById((uint)targetObjectId);
                    if (target != null) recordedActionSequence[sequence].Add(new(data, Plugin.ClientState.LocalPlayer!.Position, target.Position, Plugin.ClientState.LocalPlayer!.Rotation, false));
                    else PluginLog.Error($"SendAction: Cannot find valid target of id {targetObjectId:X} for action {actionId}");
                }
            }
        }

        // useType == 0 when queued;
        // if queued action not executed immediately but wait in queue till later, useType == 1 when called for actual execution
        private delegate byte UseActionDelegate(IntPtr actionManager, byte actionType, uint actionId, long targetObjectId, uint param, uint useType, int pvp, IntPtr a8);
        private static readonly Hook<UseActionDelegate>? UseActionHook;
        private static byte UseActionDetour(IntPtr actionManager, byte actionType, uint actionId, long targetObjectId, uint param, uint useType, int pvp, IntPtr a8)
        {
            var ret = UseActionHook!.Original(actionManager, actionType, actionId, targetObjectId, param, useType, pvp, a8);
#if DEBUG
            PluginLog.Debug($"UseAction: actionType={actionType}, actionId={actionId}, targetId={targetObjectId:X}, param={param}, useType={useType}, pvp={pvp}, a8={a8:X}; ret={ret}");
#endif
            return ret;
        }
        
        private delegate byte UseActionLocationDelegate(IntPtr actionManager, byte actionType, uint actionId, long targetObjectId, IntPtr location, uint param);
        private static readonly Hook<UseActionLocationDelegate>? UseActionLocationHook;
        private static byte UseActionLocationDetour(IntPtr actionManager, byte actionType, uint actionId, long targetObjectId, IntPtr location, uint param)
        {
            var ret = UseActionLocationHook!.Original(actionManager, actionType, actionId, targetObjectId, location, param);
#if DEBUG
            PluginLog.Debug($"UseActionLocation: actionType={actionType}, actionId={actionId}, targetId={targetObjectId:X}, loc={(Vector3)Marshal.PtrToStructure<FFXIVClientStructs.FFXIV.Client.Graphics.Vector3>(location)} param={param}; ret={ret}");
#endif
            if (ret == 0 || !Plugin.IsPlayerLoaded || !Plugin.Config.DrawGT || !ShouldProcessAction(actionType, actionId)) return ret;
            var seq = CurrentSeq;
            if (recordedActionSequence.ContainsKey(seq)) return ret;

            var originalData = ActionData.GetActionEffectRangeDataRaw(actionId);
            if (originalData == null)
            {
                PluginLog.Error($"SendAction: No excel row found for action of id {actionId}");
                return ret;
            }
#if DEBUG
            PluginLog.Debug($"---Action: id={actionId}, castType={originalData.CastType}({originalData.AoEType}), effectRange={originalData.EffectRange}, xAxisModifier={originalData.XAxisModifier}");
            PluginLog.Debug($"---Using info from UseActionLocation for GT action #{actionId}");
#endif
            var updatedEffectRangeDataSet = ActionData.CheckCornerCasesAndGetUpdatedEffectRangeData(originalData);
            if (updatedEffectRangeDataSet == null)
            {
                PluginLog.Error($"SendAction: No excel row found for action of id {actionId}");
                return ret;
            }

            foreach (var data in updatedEffectRangeDataSet)
            {
                if (!data.IsGTAction || !ShouldDrawForEffectRange(data.CastType, data.EffectRange)) continue;
                if (data.IsHarmfulAction && !Plugin.Config.DrawHarmful || !data.IsHarmfulAction && !Plugin.Config.DrawBeneficial) continue;
                if (!recordedActionSequence.ContainsKey(seq)) recordedActionSequence[seq] = new(seq);
                // Will use location from ReceiveActionEffect for target position later
                recordedActionSequence[seq].Add(new(data, Plugin.ClientState.LocalPlayer!.Position, new(), Plugin.ClientState.LocalPlayer!.Rotation)); 
            }
            return ret;
        }


        private delegate void ReceiveActionEffectDelegate(int sourceObjectId, IntPtr sourceActor, IntPtr position, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail);
        private static readonly Hook<ReceiveActionEffectDelegate>? ReceiveActionEffectHook;
        private static void ReceiveActionEffectDetour(int sourceObjectId, IntPtr sourceActor, IntPtr position, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail)
        {
            ReceiveActionEffectHook!.Original(sourceObjectId, sourceActor, position, effectHeader, effectArray, effectTrail);
#if DEBUG
            PluginLog.Debug($"ReceiveActionEffect: src={sourceObjectId:X}, pos={(Vector3)Marshal.PtrToStructure<FFXIVClientStructs.FFXIV.Client.Graphics.Vector3>(position)}; AcMgr: CurrentSeq={CurrentSeq}, LastRecSeq={LastRecievedSeq}");
#endif

            if (!Plugin.IsPlayerLoaded) return;

            if (effectHeader == IntPtr.Zero)
            {
                PluginLog.Error("ReceiveActionEffect: effectHeader ptr is zero");
                return;
            }
            var header = Marshal.PtrToStructure<ActionEffectHeader>(effectHeader);
#if DEBUG
            PluginLog.Debug($"---effectHeader: target={header.TargetObjectId:X}, action={header.ActionId}, unkObjId={header.UnkObjectId:X}, seq={header.Sequence}, unk={header.Unk_1A:X}");
#endif

            if (header.Sequence == 0 && sourceObjectId == Plugin.BuddyList.PetBuddy?.GameObject?.ObjectId)
            {
                if (recordedPetLikeActionEffectToWait.TryGetValue(header.ActionId, out var petLikeSeqInfos))
                {
                    foreach (var info in petLikeSeqInfos)
                    {
                        EffectRangeDrawing.AddEffectRangeToDraw(info.EffectRangeData, info.OriginPosition,
                            info.EffectRangeData.IsGTAction ? Marshal.PtrToStructure<FFXIVClientStructs.FFXIV.Client.Graphics.Vector3>(position) : info.TargetPosition,
                            info.ActorRotation);
                    }
                    recordedPetLikeActionEffectToWait.Remove(header.ActionId);
                }
                if (Plugin.Config.DrawOwnPets && recordedPetActionEffectToWait.TryGetValue(header.ActionId, out var petSeqInfos))
                {
                    foreach (var info in petSeqInfos)
                    {
                        EffectRangeDrawing.AddEffectRangeToDraw(info.EffectRangeData, info.OriginPosition,
                            info.EffectRangeData.IsGTAction ? Marshal.PtrToStructure<FFXIVClientStructs.FFXIV.Client.Graphics.Vector3>(position) : info.TargetPosition,
                            info.ActorRotation);
                    }
                    recordedPetLikeActionEffectToWait.Remove(header.ActionId);
                }
            }
            else if (recordedActionSequence.TryGetValue(header.Sequence, out var seqInfos))
            {
                foreach (var info in seqInfos)
                {
                    EffectRangeDrawing.AddEffectRangeToDraw(info.EffectRangeData, info.OriginPosition,
                            info.EffectRangeData.IsGTAction ? Marshal.PtrToStructure<FFXIVClientStructs.FFXIV.Client.Graphics.Vector3>(position) : info.TargetPosition,
                            info.ActorRotation);
                }
                recordedActionSequence.Remove(header.Sequence);
            }
        }


        private static ushort CurrentSeq => actionMgrPtr != IntPtr.Zero ? (ushort)Marshal.ReadInt16(actionMgrPtr + 0x110) : (ushort)0;
        private static ushort LastRecievedSeq => actionMgrPtr != IntPtr.Zero ? (ushort)Marshal.ReadInt16(actionMgrPtr + 0x112) : (ushort)0;

        
        private static readonly Dictionary<ushort, ActionSequenceInfoSet> recordedActionSequence = new();
        private static readonly Dictionary<uint, ActionSequenceInfoSet> recordedPetActionEffectToWait = new();
        private static readonly Dictionary<uint, ActionSequenceInfoSet> recordedPetLikeActionEffectToWait = new();

        private static void ClearActionSequenceInfoCache()
        {
            recordedActionSequence.Clear();
            recordedPetActionEffectToWait.Clear();
            recordedPetLikeActionEffectToWait.Clear();
        }


        private static bool ShouldDrawForActionType(uint actionType) => actionType == 0x1 || actionType == 0xE; // pve 0x1, pvp 0xE

        // TODO: also check blacklist
        private static bool ShouldDrawForAction(uint actionId) => !ActionData.IsRuledOutAction(actionId);

        // Only check for circle (2) and donut (10)
        private static bool ShouldDrawForEffectRange(byte castType, byte effectRange) =>
            effectRange > 0 && (!(castType == 2 || castType == 10) || (Plugin.Config.LargeDrawOpt != 1 || effectRange < Plugin.Config.LargeThreshold));

        private static bool ShouldProcessAction(byte actionType, uint actionId)
            => Plugin.IsPlayerLoaded && ShouldDrawForActionType(actionType) && ShouldDrawForAction(actionId);


        private static uint playerClassJob;

        private static void OnUpdateClearCacheOnJobChange(Dalamud.Game.Framework _)
        {
            if (!Plugin.IsPlayerLoaded) return;
            var currentClassJob = Plugin.ClientState.LocalPlayer!.ClassJob.Id;
            if (playerClassJob != currentClassJob)
            {
                ClearActionSequenceInfoCache();
                playerClassJob = Plugin.ClientState.LocalPlayer.ClassJob.Id;
            }
        }

        private static void OnTerritoryChangedClearCache(object? sender, ushort terr)
        {
            ClearActionSequenceInfoCache();
        }


        static ActionWatcher()
        {
            unsafe
            {
                actionMgrPtr = (IntPtr)ActionManager.Instance();
            }

#if DEBUG
            UseActionHook ??= new Hook<UseActionDelegate>(Plugin.SigScanner.ScanText("E8 ?? ?? ?? ?? 89 9F 14 79 02 00"), UseActionDetour);
#endif
            UseActionLocationHook ??= new Hook<UseActionLocationDelegate>(Plugin.SigScanner.ScanText("E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? 81 FB FB 1C 00 00"), UseActionLocationDetour);
            ReceiveActionEffectHook ??= new Hook<ReceiveActionEffectDelegate>(Plugin.SigScanner.ScanText("E8 ?? ?? ?? ?? 48 8B 8D F0 03 00 00"), ReceiveActionEffectDetour);
            SendActionHook ??= new Hook<SendActionDelegate>(Plugin.SigScanner.ScanText("E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? F3 0F 10 3D ?? ?? ?? ?? 48 8D 4D BF"), SendActionDetour);
        }

        public static void Enable()
        {
#if DEBUG
            UseActionHook?.Enable();
#endif
            UseActionLocationHook?.Enable();
            SendActionHook?.Enable();
            ReceiveActionEffectHook?.Enable();

            Plugin.Framework.Update += OnUpdateClearCacheOnJobChange;
            Plugin.ClientState.TerritoryChanged += OnTerritoryChangedClearCache;
        }

        public static void Disable()
        {
#if DEBUG
            UseActionHook?.Disable();
#endif
            UseActionLocationHook?.Disable();
            SendActionHook?.Disable();
            ReceiveActionEffectHook?.Disable();

            Plugin.Framework.Update -= OnUpdateClearCacheOnJobChange;
            Plugin.ClientState.TerritoryChanged -= OnTerritoryChangedClearCache;
        }

        public static void Dispose()
        {
            Disable();
#if DEBUG
            UseActionHook?.Dispose();
#endif
            UseActionLocationHook?.Dispose();
            SendActionHook?.Dispose();
            ReceiveActionEffectHook?.Dispose();
        }

    }


    [StructLayout(LayoutKind.Explicit)]
    public struct ActionEffectHeader
    {
        [FieldOffset(0x0)] public long TargetObjectId;
        [FieldOffset(0x8)] public uint ActionId;
        // Unk; but have some value keep accumulating here
        [FieldOffset(0x14)] public uint UnkObjectId;
        [FieldOffset(0x18)] public ushort Sequence; // Corresponds exactly to the sequence of the action used; AA, pet's action effect etc. will be 0 here
        [FieldOffset(0x1A)] public ushort Unk_1A;   // Seems related to SendAction's arg a5, but not always the same value
        // rest??
    }
}
