using ActionEffectRange.Drawing;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Hooking;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace ActionEffectRange.Actions
{
    public static class ActionWatcher
    {
        private static readonly IntPtr actionMgrPtr;

        //private delegate byte CanUseActionOnGameObjectDelegate(uint actionId, IntPtr gameObj);
        //private static readonly CanUseActionOnGameObjectDelegate? canUseActionOnGameObject;

        // SendAction returned before UseAction
        // Information here is more accurate; solves the prob with procs etc. and with plugins like FFXIVCombo / ReAction 
        // Not called for ground targets
        private delegate void SendActionDelegate(long targetObjectId, byte actionType, uint actionId, ushort sequence, long a5, long a6, long a7, long a8, long a9);
        private static readonly Hook<SendActionDelegate>? SendActionHook;
        private static void SendActionDetour(long targetObjectId, byte actionType, uint actionId, ushort sequence, long a5, long a6, long a7, long a8, long a9)
        {
            SendActionHook!.Original(targetObjectId, actionType, actionId, sequence, a5, a6, a7, a8, a9);
#if DEBUG
            PluginLog.Debug($"SendAction: targetId={targetObjectId:X}, actionType={actionType}, actionId={actionId}, seq={sequence}, a5={a5:X}, a6={a6:X}, a7={a7:X}, a8={a8:X}, a9={a9:X}");
            PluginLog.Debug($"---AcMgr: currentSeq{CurrentSeq}, lastRecSeq={LastRecievedSeq}");
#endif

            if (!Plugin.IsPlayerLoaded) return;
            
            if (!ActionValidator.ShouldDrawForActionType(actionType)) return;

            // TODO: check black list

            var origEffectRangeData = ActionData.GetActionEffectRangeData(actionId);

            if (origEffectRangeData == null)
            {
                PluginLog.Error($"SendAction: No excel row found for action of id {actionId}");
                return;
            }
#if DEBUG
            PluginLog.Debug($"---Action: id={actionId}, castType={origEffectRangeData.CastType}({origEffectRangeData.AoEType}), effectRange={origEffectRangeData.EffectRange}, xAxisModifier={origEffectRangeData.XAxisModifier}");
#endif

            // Check & update/replace if necessary
            var updatedEffectRangeDataSet = ActionValidator.UpdateEffectRangeData(origEffectRangeData);
            foreach (var data in updatedEffectRangeDataSet)
            {
#if DEBUG
                PluginLog.Debug($"---updated effect range data for action #{actionId}");
#endif
                if (Plugin.Config.DrawBeneficial && !data.IsHarmfulAction || Plugin.Config.DrawHarmful && data.IsHarmfulAction)
                {
                    if (!lastRecordedAwaitedActionDataMap.ContainsKey(data.ActionId)) lastRecordedAwaitedActionDataMap[data.ActionId] = new();

                    if (data.Range == 0)
                        lastRecordedAwaitedActionDataMap[data.ActionId].Add((data, Plugin.ClientState.LocalPlayer!.Position));
                    else
                    {
                        var target = Plugin.ObejctTable.SearchById((uint)targetObjectId);
                        if (target != null) lastRecordedAwaitedActionDataMap[data.ActionId].Add((data, target.Position));
                        else PluginLog.Error($"SendAction: Cannot find valid target of id {targetObjectId:X} for action {actionId}");
                    }
                }
            }

            if (Plugin.Config.DrawOwnPets && ActionValidator.CheckPetAction(actionId, out var petActionEffectRangeDataSet) 
                && petActionEffectRangeDataSet != null)
            {
                var pet = Plugin.BuddyList.PetBuddy?.GameObject;
                if (pet != null)
                {
#if DEBUG
                    PluginLog.Debug($"---check pet action: pet objId={pet.ObjectId:X}, pos={pet.Position}");
#endif
                    foreach (var data in petActionEffectRangeDataSet)
                    {
                        if (data == null) continue;
                        if (!lastRecordedAwaitedActionDataMap.ContainsKey(data.ActionId)) lastRecordedAwaitedActionDataMap[data.ActionId] = new();
                        if (Plugin.Config.DrawBeneficial && !data.IsHarmfulAction
                            || Plugin.Config.DrawHarmful && data.IsHarmfulAction)
                        {
                            if (data.Range == 0)
                                lastRecordedAwaitedActionDataMap[data.ActionId].Add((data, pet.Position));
                            else
                            {
                                var target = Plugin.ObejctTable.SearchById((uint)targetObjectId);
                                if (target != null) lastRecordedAwaitedActionDataMap[data.ActionId].Add((data, target.Position));
                                else PluginLog.Error($"SendAction: Cannot find valid target of id {targetObjectId:X} for action {actionId}");
                            }
#if DEBUG
                            PluginLog.Debug($"---Found possible pet action: actionId={data.ActionId}");
#endif
                        }
                    }
                }
            }

            lastRecordedSeq = lastRecordedAwaitedActionDataMap.Any() ? sequence : (ushort)0;
        }

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

        
        private delegate void ReceiveActionEffectDelegate(int sourceObjectId, IntPtr sourceActor, IntPtr vectorPosition, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail);
        private static readonly Hook<ReceiveActionEffectDelegate>? ReceiveActionEffectHook;
        private static void ReceiveActionEffectDetour(int sourceObjectId, IntPtr sourceActor, IntPtr vectorPosition, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail)
        {
            ReceiveActionEffectHook!.Original(sourceObjectId, sourceActor, vectorPosition, effectHeader, effectArray, effectTrail);
#if DEBUG
            PluginLog.Debug($"ReceiveActionEffect: src={sourceObjectId:X}; AcMgr: CurrentSeq={CurrentSeq}, LastRecSeq={LastRecievedSeq}");
#endif

            if (!lastRecordedAwaitedActionDataMap.Any() || lastRecordedSeq == 0) return;

            if (effectHeader == IntPtr.Zero)
            {
                PluginLog.Error("ReceiveActionEffect: effectHeader ptr is zero");
                return;
            }
            var header = Marshal.PtrToStructure<ActionEffectHeader>(effectHeader);
#if DEBUG
            PluginLog.Debug($"---effectHeader: target={header.TargetObjectId:X}, action={header.ActionId}, unkObjId={header.UnkObjectId:X}, seq={header.Sequence}, unk={header.Unk_1A:X}");
#endif

            if (header.Sequence > lastRecordedSeq)
            {
                lastRecordedAwaitedActionDataMap.Clear();
                lastRecordedSeq = 0;
            }


            if (header.Sequence == lastRecordedSeq
                || header.Sequence == 0 && sourceObjectId == Plugin.BuddyList.PetBuddy?.GameObject?.ObjectId)
                if (lastRecordedAwaitedActionDataMap.TryGetValue(header.ActionId, out var dataSet))
                {
                    foreach (var dataPair in dataSet)
                        EffectRangeDrawing.AddEffectRangeToDraw(dataPair.EffectRangeData, dataPair.TargetPosition);
                    lastRecordedAwaitedActionDataMap.Clear();
                    lastRecordedSeq = 0;
                }
        }


        private static ushort CurrentSeq => (ushort)Marshal.ReadInt16(actionMgrPtr + 0x110);
        private static ushort LastRecievedSeq => (ushort)Marshal.ReadInt16(actionMgrPtr + 0x112);


        private static readonly Dictionary<uint, HashSet<(EffectRangeData EffectRangeData, Vector3 TargetPosition)>>
            lastRecordedAwaitedActionDataMap = new();
        private static ushort lastRecordedSeq;


        //private static bool CanUseActionOnTarget(uint actionId, IntPtr target) => canUseActionOnGameObject?.Invoke(actionId, target) > 0;

        
        static ActionWatcher()
        {
            unsafe
            {
                actionMgrPtr = (IntPtr)ActionManager.Instance();
            }

            //canUseActionOnGameObject ??= Marshal.GetDelegateForFunctionPointer<CanUseActionOnGameObjectDelegate>(Plugin.SigScanner.ScanText("48 89 5C 24 08 57 48 83 EC 20 48 8B DA 8B F9 E8 ?? ?? ?? ?? 4C 8B C3"));

            UseActionHook ??= new Hook<UseActionDelegate>(Plugin.SigScanner.ScanText("E8 ?? ?? ?? ?? 89 9F 14 79 02 00"), UseActionDetour);
            // UseActionLocation
            ReceiveActionEffectHook ??= new Hook<ReceiveActionEffectDelegate>(Plugin.SigScanner.ScanText("E8 ?? ?? ?? ?? 48 8B 8D F0 03 00 00"), ReceiveActionEffectDetour);
            SendActionHook ??= new Hook<SendActionDelegate>(Plugin.SigScanner.ScanText("E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? F3 0F 10 3D ?? ?? ?? ?? 48 8D 4D BF"), SendActionDetour);
        }

        public static void Enable()
        {
            UseActionHook?.Enable();
            ReceiveActionEffectHook?.Enable();
            SendActionHook?.Enable();
        }

        public static void Disable()
        {
            UseActionHook?.Disable();
            ReceiveActionEffectHook?.Disable();
            SendActionHook?.Disable();
        }

        public static void Dispose()
        {
            Disable();
            UseActionHook?.Dispose();
            ReceiveActionEffectHook?.Dispose();
            SendActionHook?.Dispose();
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
