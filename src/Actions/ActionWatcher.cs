using ActionEffectRange.Actions.Data;
using ActionEffectRange.Actions.EffectRange;
using ActionEffectRange.Drawing;
using ActionEffectRange.Helpers;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Hooking;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Vector3Struct = FFXIVClientStructs.FFXIV.Common.Math.Vector3;

namespace ActionEffectRange.Actions
{
    internal static class ActionWatcher
    {
        private const float SeqExpiry = 2.5f; // this is arbitrary...

        private static uint lastSendActionSeq = 0;
        private static uint lastReceivedMainSeq = 0;

        private static readonly ActionSeqRecord playerActionSeqs = new(5);
        private static readonly HashSet<ushort> skippedSeqs = new();

        // Send what is executed; won't be called if queued but not yet executed
        //  or failed to execute (e.g. cast cancelled)
        // Information here also more accurate than from UseAction; handles combo/proc
        //  and target issues esp. with other plugins being used.
        // Not called for GT actions
        private delegate void SendActionDelegate(long targetObjectId, 
            byte actionType, uint actionId, ushort sequence, long a5, long a6, long a7, long a8, long a9);
        private static readonly Hook<SendActionDelegate>? SendActionHook;
        private static void SendActionDetour(long targetObjectId, 
            byte actionType, uint actionId, ushort sequence, long a5, long a6, long a7, long a8, long a9)
        {
            SendActionHook!.Original(targetObjectId, actionType, actionId, sequence, a5, a6, a7, a8, a9);

            try
            {
                LogUserDebug($"SendAction => target={targetObjectId:X}, " +
                    $"action={actionId}, type={actionType}, seq={sequence}");
#if DEBUG
                LogDebug($"** SendAction: targetId={targetObjectId:X}, " +
                    $"actionType={actionType}, actionId={actionId}, seq={sequence}, " +
                    $"a5={a5:X}, a6={a6:X}, a7={a7:X}, a8={a8:X}, a9={a9:X}");
                LogDebug($"** ---AcMgr: currentSeq{ActionManagerHelper.CurrentSeq}, " +
                    $"lastRecSeq={ActionManagerHelper.LastRecievedSeq}");
#endif
                lastSendActionSeq = sequence;

                if (!ShouldProcessAction(actionType, actionId))
                {
                    skippedSeqs.Add(sequence);
                    return;
                }

                var actionCategory = ActionData.GetActionCategory(actionId);
                if (!ShouldDrawForActionCategory(actionCategory))
                {
                    LogUserDebug($"---Skip action#{actionId}: " +
                        $"Not drawing for actions of category {actionCategory}");
                    skippedSeqs.Add(sequence);
                    return;
                }

                if (targetObjectId == 0 || targetObjectId == InvalidGameObjectId)
                {
                    LogUserDebug($"---Skip: Invalid target #{targetObjectId}");
                    return;
                }
                else if (targetObjectId == LocalPlayer!.ObjectId)
                {
                    var snapshot = new SeqSnapshot(sequence);
                    playerActionSeqs.Add(new(actionId, snapshot, false));
                }
                else
                {
                    var target = ObejctTable.SearchById((uint)targetObjectId);
                    if (target != null)
                    {
                        var snapshot = new SeqSnapshot(sequence,
                            target.ObjectId, target.Position);
                        playerActionSeqs.Add(new(actionId, snapshot, false));
                    }
                    else
                    {
                        LogError($"Cannot find valid target #{targetObjectId:X} for action#{actionId}");
                        return;
                    }
                }
            } 
            catch (Exception e)
            {
                LogError($"{e}");
            }
        }

        private delegate byte UseActionLocationDelegate(IntPtr actionManager, 
            byte actionType, uint actionId, long targetObjectId, IntPtr location, uint param);
        private static readonly Hook<UseActionLocationDelegate>? UseActionLocationHook;
        private static byte UseActionLocationDetour(IntPtr actionManager, 
            byte actionType, uint actionId, long targetObjectId, IntPtr location, uint param)
        {
            var ret = UseActionLocationHook!.Original(actionManager, 
                actionType, actionId, targetObjectId, location, param);
            try
            {
#if DEBUG
                LogDebug($"** UseActionLocation: actionType={actionType}, " +
                    $"actionId={actionId}, targetId={targetObjectId:X}, " +
                    $"loc={location:X} " +
                    $"=> {(Vector3)Marshal.PtrToStructure<Vector3Struct>(location)} " +
                    $"param={param}; ret={ret}");
#endif
                if (ret == 0) return ret;

                var seq = ActionManagerHelper.CurrentSeq;

                // Skip if already processed in SendAction; these are not GT actions
                if (seq == lastSendActionSeq) return ret;

                LogUserDebug($"UseActionLocation => " +
                    $"Possible GT action #{actionId}, type={actionType};" +
                    $"Seq={ActionManagerHelper.CurrentSeq}");

                if (!Config.DrawGT)
                {
                    LogUserDebug($"---Skip: Config: disabed for GT actions");
                    skippedSeqs.Add(seq);
                    return ret;
                }

                if (!ShouldProcessAction(actionType, actionId))
                {
                    skippedSeqs.Add(seq);
                    return ret;
                }

                var actionCategory = ActionData.GetActionCategory(actionId);
                if (!ShouldDrawForActionCategory(actionCategory))
                {
                    LogUserDebug($"---Skip action#{actionId}: " +
                        $"Not drawing for actions of category {actionCategory}");
                    skippedSeqs.Add(seq);
                    return ret;
                }

                // NOTE: Should've checked if the action could be mapped to some pet/pet-like actions
                // but currently none for those actions if we've reached here so just omit it for now

                playerActionSeqs.Add(new(actionId, new(seq), false));
            }
            catch (Exception e)
            {
                LogError($"{e}");
            }
            return ret;
        }

        // useType == 0 when queued;
        // If queued action not executed immediately,
        //  useType == 1 when this function is called later to actually execute the action
        private delegate byte UseActionDelegate(IntPtr actionManager, 
            byte actionType, uint actionId, long targetObjectId, uint param, uint useType, int pvp, IntPtr a8);
        private static readonly Hook<UseActionDelegate>? UseActionHook;
        // Detour used mainly for processing draw-when-casting
        // When applicable, drawing is triggered immediately
        private static byte UseActionDetour(IntPtr actionManager, 
            byte actionType, uint actionId, long targetObjectId, uint param, uint useType, int pvp, IntPtr a8)
        {
            var ret = UseActionHook!.Original(actionManager, 
                actionType, actionId, targetObjectId, param, useType, pvp, a8);

            try
            {
                LogUserDebug($"UseAction => actionType={actionType}, " +
                    $"actionId={actionId}, targetId={targetObjectId:X}");
#if DEBUG
                LogDebug($"** UseAction: param={param}, useType={useType}, pvp={pvp}, a8={a8:X}; " +
                    $"ret={ret}; CurrentSeq={ActionManagerHelper.CurrentSeq}");
#endif
                if (!DrawWhenCasting) return ret;

                if (!ActionManagerHelper.IsCasting)
                {
                    LogUserDebug($"---Skip: not casting");
                    return ret;
                }

                if (ret == 0)
                {
                    LogUserDebug($"---Skip: not drawing on useType={useType} && ret={ret}");
                    return ret;
                }

                var castActionId = ActionManagerHelper.CastingActionId;

                if (!ShouldProcessAction(actionType, castActionId))
                    return ret;

                var actionIdsToDraw = new List<uint> { castActionId };
                if (ActionData.TryGetActionWithAdditionalEffects(
                    castActionId, out var additionals))
                    actionIdsToDraw.AddRange(additionals);

                foreach (var a in actionIdsToDraw)
                {
                    var erdata = EffectRangeDataManager.NewData(a);

                    if (erdata == null)
                    {
                        LogError($"Cannot get data for action#{a}");
                        continue;
                    }

                    if (!ShouldDrawForActionCategory(erdata.Category, true))
                    {
                        LogUserDebug($"---Skip action#{erdata.ActionId}: " +
                            $"Not drawing for actions of category {erdata.Category}");
                        continue;
                    }

                    erdata = EffectRangeDataManager.CustomiseEffectRangeData(erdata);
                    if (!CheckShouldDrawPostCustomisation(erdata)) continue;

                    var seq = ActionManagerHelper.CurrentSeq;
                    float rotation = ActionManagerHelper.CastRotation;

                    if (erdata.IsGTAction)
                    {
                        var targetPos = new Vector3(
                            ActionManagerHelper.CastTargetPosX,
                            ActionManagerHelper.CastTargetPosY,
                            ActionManagerHelper.CastTargetPosZ);
                        LogUserDebug($"UseAction => Triggering draw-when-casting, " +
                            $"CastingActionId={castActionId}, GT action, " +
                            $"CastPosition={targetPos}, CastRotation={rotation}");
                        LogUserDebug($"---Adding DrawData for action #{castActionId} " +
                            $"from player, using cast position info");
                        EffectRangeDrawing.AddEffectRangeToDraw(seq,
                            DrawTrigger.Casting, erdata, LocalPlayer!.Position,
                            targetPos, rotation);
                    }
                    else
                    {
                        var castTargetId = ActionManagerHelper.CastTargetObjectId;
                        LogUserDebug($"UseAction => Triggering draw-when-casting, " +
                            $"CastingActionId={castActionId}, " +
                            $"CastTargetObjectId={castTargetId}, CastRotation={rotation}");

                        GameObject? target = null;
                        if (castTargetId == LocalPlayer!.ObjectId)
                            target = LocalPlayer;
                        else if (castTargetId != 0 
                            && castTargetId != InvalidGameObjectId)
                            target = ObejctTable.SearchById(castTargetId);

                        if (target != null)
                        {
                            LogUserDebug($"---Adding DrawData for action #{castActionId} " +
                                $"from player, using cast position info");
                            // We do not have GT actions here
                            EffectRangeDrawing.AddEffectRangeToDraw(
                                ActionManagerHelper.CurrentSeq, DrawTrigger.Casting, 
                                erdata, LocalPlayer!.Position, target.Position, rotation);
                        }
                        else LogUserDebug($"---Failed: Target #{castTargetId:X} not found");
                    }
                }
            }
            catch (Exception e)
            {
                LogError($"{e}");
            }

            return ret;
        }

        
        private delegate void ReceiveActionEffectDelegate(int sourceObjectId, IntPtr sourceActor, 
            IntPtr position, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail);
        private static readonly Hook<ReceiveActionEffectDelegate>? ReceiveActionEffectHook;

        private static void ReceiveActionEffectDetour(int sourceObjectId, IntPtr sourceActor, 
            IntPtr position, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail)
        {
            ReceiveActionEffectHook!.Original(sourceObjectId, sourceActor, 
                position, effectHeader, effectArray, effectTrail);

            try
            {
#if DEBUG
                LogDebug($"** ReceiveActionEffect: src={sourceObjectId:X}, " +
                    $"pos={(Vector3)Marshal.PtrToStructure<Vector3Struct>(position)}; " +
                    $"AcMgr: CurrentSeq={ActionManagerHelper.CurrentSeq}, " +
                    $"LastRecSeq={ActionManagerHelper.LastRecievedSeq}");
#endif

                if (effectHeader == IntPtr.Zero)
                {
                    LogError("ReceiveActionEffect: effectHeader ptr is zero");
                    return;
                }
                var header = Marshal.PtrToStructure<ActionEffectHeader>(effectHeader);
                LogUserDebug($"ReceiveActionEffect => " +
                    $"source={sourceObjectId:X}, target={header.TargetObjectId:X}, " +
                    $"action={header.ActionId}, seq={header.Sequence}");
#if DEBUG
                LogDebug($"** ---effectHeader: target={header.TargetObjectId:X}, " +
                    $"action={header.ActionId}, unkObjId={header.UnkObjectId:X}, " +
                    $"seq={header.Sequence}, unk={header.Unk_1A:X}");
#endif

                if (header.Sequence > 0)
                {
                    lastReceivedMainSeq = header.Sequence;
                    if (skippedSeqs.Contains(header.Sequence))
                    {
                        LogUserDebug($"---Skip: not processing Seq#{header.Sequence}");
                        return;
                    }
                }

                if (!IsPlayerLoaded)
                {
                    LogUserDebug($"---Skip: PC not loaded");
                    return;
                }

                if (sourceObjectId != LocalPlayer!.ObjectId
                    && (!PetWatcher.HasPetPresent 
                        || PetWatcher.GetPetObjectId() != sourceObjectId))
                {
                    LogUserDebug($"---Skip: Effect triggered by others");
                    return;
                }

                var erdata = EffectRangeDataManager.NewData(header.ActionId);
                if (erdata == null)
                {
                    LogError($"Cannot get data for action#{header.ActionId}");
                    return;
                }

                // Some additional effects (e.g. #29706 additional effect for Pneuma pvp)
                // have ActionCategory=0
                if (!ShouldDrawForActionCategory(erdata.Category, true))
                {
                    LogUserDebug($"---Skip action#{erdata.ActionId}: " +
                        $"Not drawing for actions of category {erdata.Category}");
                    return;
                }

                erdata = EffectRangeDataManager.CustomiseEffectRangeData(erdata);

                if (!CheckShouldDrawPostCustomisation(erdata)) return;

                var mainSeq = header.Sequence > 0
                        ? header.Sequence : lastReceivedMainSeq;

                if (sourceObjectId == LocalPlayer!.ObjectId)
                {
                    // Source is pc

                    // TODO: config on/off: auto triggered effects
                    //  (such as effects on time elapsed, receiving damage, ...)
                    bool drawForAuto = true; // placeholder

                    ActionSeqInfo? seqInfo = null;

                    // For additional effects, received data always has seq=0
                    // Match seq using predefined mapping and some heuristics
                    if (!ActionData.ShouldNotUseCachedSeq(header.ActionId))
                        seqInfo = FindRecordedSeqInfo(header.Sequence, header.ActionId);

                    if (seqInfo != null)
                    {
                        // Additional effect may have different target (e.g. self vs targeted enemy)
                        Vector3 targetPos = erdata.IsGTAction
                            ? Marshal.PtrToStructure<Vector3Struct>(position)
                            : (header.TargetObjectId == LocalPlayer.ObjectId
                                ? seqInfo.SeqSnapshot.PlayerPosition
                                : seqInfo.SeqSnapshot.TargetPosition);

                        LogUserDebug($"---Adding DrawData for action #{header.ActionId} " +
                            $"from player, using SeqSnapshot#{seqInfo.Seq}");
                        EffectRangeDrawing.AddEffectRangeToDraw(seqInfo.Seq,
                            DrawTrigger.Used, erdata, seqInfo.SeqSnapshot.PlayerPosition,
                            targetPos, seqInfo.SeqSnapshot.PlayerRotation);
                    }
                    else if (drawForAuto)
                    {
                        LogUserDebug($"---Adding DrawData for action #{header.ActionId} " +
                            $"from player, using current position info");

                        if (erdata.IsGTAction)
                            EffectRangeDrawing.AddEffectRangeToDraw(mainSeq,
                                DrawTrigger.Used, erdata, LocalPlayer!.Position,
                                Marshal.PtrToStructure<Vector3Struct>(position),
                                LocalPlayer!.Rotation);
                        else
                        {
                            GameObject? target = null;
                            if (header.TargetObjectId == sourceObjectId) // Self-targeting
                                target = LocalPlayer;
                            else if (header.TargetObjectId != 0
                                && header.TargetObjectId != InvalidGameObjectId)
                                target = ObejctTable.SearchById((uint)header.TargetObjectId);

                            if (target != null)
                            {
                                EffectRangeDrawing.AddEffectRangeToDraw(mainSeq,
                                    DrawTrigger.Used, erdata, LocalPlayer!.Position,
                                    target.Position, LocalPlayer!.Rotation);
                            }
                            else LogUserDebug($"---Failed: Target #{header.TargetObjectId:X} not found");
                        }
                    }
                    else LogUserDebug($"---Skip: Not drawing for auto-triggered action #{header.ActionId}");
                }
                else
                {
                    // Source may be player's pet/pet-like object

                    // NOTE: Always use current position infos here.
                    // Due to potential delay, info snapshot at the time
                    //  player action is used is not accurate for pet actions as well.
                    // E.g., when pet is moving, any other action will be delayed;
                    //  once the pet is settled on a location, the character positions
                    //  are snapshot and pet action is processed based on this snapshot;
                    // but this also means the snapshot produced when player used
                    //  the "parent" action is already out-of-date.

                    // Just ignore if the pet is no longer present at this point (e.g. due to delay).
                    // Not very common as the game already defers removing pet objects
                    //  possibly to account for delays
                    if (PetWatcher.HasPetPresent
                        && PetWatcher.GetPetObjectId() == sourceObjectId)
                    {
                        if (PetWatcher.IsCurrentPetACNPet() && !Config.DrawACNPets)
                        {
                            LogUserDebug($"---Skip: Drawing for action#{header.ActionId} " +
                                "from ACN/SMN/SCH pets configured OFF");
                            return;
                        }
                        if (PetWatcher.IsCurrentPetNonACNNamedPet()
                            && !Config.DrawSummonedCompanions)
                        {
                            LogUserDebug($"---Skip: Drawing for action#{header.ActionId} " +
                                "from summoned companions of non-ACN based jobs configured OFF");
                            return;
                        }
                        if (PetWatcher.IsCurrentPetNameless()
                            && !Config.DrawGT)
                        {
                            // Assuming all nameless pets are ground placed objects ...
                            LogUserDebug($"---Skip: Drawing for action#{header.ActionId} " +
                                "from possibly ground placed object configured OFF");
                            return;
                        }

                        // TODO: Check if the effect is auto-triggered if it is from placed object?
                        // (Assuming placed obj does not move, cached seq snapshot can be used.)
                        // (Configurable opt)

                        LogUserDebug($"---Add DrawData for action #{header.ActionId} " +
                            $"from pet / pet-like object #{sourceObjectId:X}, using current position info");

                        if (erdata.IsGTAction)
                            EffectRangeDrawing.AddEffectRangeToDraw(mainSeq,
                                DrawTrigger.Used, erdata, LocalPlayer!.Position,
                                Marshal.PtrToStructure<Vector3Struct>(position),
                                LocalPlayer!.Rotation);
                        else
                        {
                            GameObject? target = null;
                            if (header.TargetObjectId == sourceObjectId) // Pet self-targeting
                                target = PetWatcher.GetPet();
                            else if (header.TargetObjectId == LocalPlayer.ObjectId)
                                target = LocalPlayer;
                            else if (header.TargetObjectId != 0
                                && header.TargetObjectId != InvalidGameObjectId)
                                target = ObejctTable.SearchById((uint)header.TargetObjectId);

                            if (target != null)
                            {
                                var source = PetWatcher.GetPet();
                                EffectRangeDrawing.AddEffectRangeToDraw(mainSeq,
                                    DrawTrigger.Used, erdata,
                                    PetWatcher.GetPetPosition(),
                                    target.Position, PetWatcher.GetPetRotation());
                            }
                            else LogUserDebug($"---Failed: Target #{header.TargetObjectId:X} not found");
                        }
                    }
                    else LogUserDebug($"---Skip: source actor #{sourceObjectId:X} not matching pc or pet");
                }
            }
            catch (Exception e)
            {
                LogError($"{e}");
            }
        }


        #region Checks

        private static bool ShouldProcessAction(byte actionType, uint actionId)
        {
            if (!IsPlayerLoaded)
            {
                LogUserDebug($"---Skip: PC not loaded");
                return false;
            }
            if (!ShouldProcessActionType(actionType) 
                || !ShouldProcessAction(actionId))
            {
                LogUserDebug($"---Skip: Not processing " +
                    $"action#{actionId}, ActionType={actionType}");
                return false;
            }
            return true;
        }

        private static bool ShouldProcessActionType(uint actionType) 
            => actionType == 0x1 || actionType == 0xE; // pve 0x1, pvp 0xE

        private static bool ShouldProcessAction(uint actionId)
            => !ActionData.IsActionBlacklisted(actionId);


        private static bool ShouldDrawForActionCategory(
            Enums.ActionCategory actionCategory, bool allowCateogry0 = false)
            => ActionData.IsCombatActionCategory(actionCategory)
            || Config.DrawEx && ActionData.IsSpecialOrArtilleryActionCategory(actionCategory)
            || allowCateogry0 && actionCategory == 0;

        // Only check for circle and donut in Large EffectRange check
        private static bool ShouldDrawForEffectRange(EffectRangeData data)
            => data.EffectRange > 0 
            && (!(data is CircleAoEEffectRangeData || data is DonutAoEEffectRangeData) 
                || Config.LargeDrawOpt != 1 
                || data.EffectRange < Config.LargeThreshold);

        // Note: will not draw for `None` (=0)
        private static bool ShouldDrawForHarmfulness(EffectRangeData data)
            => EffectRangeDataManager.IsHarmfulAction(data) && Config.DrawHarmful
            || EffectRangeDataManager.IsBeneficialAction(data) && Config.DrawBeneficial;


        private static bool CheckShouldDrawPostCustomisation(EffectRangeData data)
        {
            if (!ShouldDrawForEffectRange(data))
            {
                LogUserDebug($"---Skip action #{data.ActionId}: " +
                    $"Not drawing for actions of effect range = {data.EffectRange}");
                return false;
            }

            if (!ShouldDrawForHarmfulness(data))
            {
                LogUserDebug($"---Skip action #{data.ActionId}: " +
                    $"Not drawing for harmful/beneficial actions = {data.Harmfulness}");
                return false;
            }

            return true;
        }

        #endregion


        private static ActionSeqInfo? FindRecordedSeqInfo(
            ushort receivedSeq, uint receivedActionId)
        {
            foreach (var seqInfo in playerActionSeqs)
            {
                if (IsSeqExpired(seqInfo)) continue;
                if (receivedSeq > 0) // Primary effects from player actions
                {
                    if (receivedSeq == seqInfo.Seq)
                    {
                        LogUserDebug($"---* Recorded sequence matched");
                        return seqInfo;
                    }
                }
                else if (ActionData.AreRelatedPlayerTriggeredActions(
                    seqInfo.ActionId, receivedActionId))
                {
                    LogUserDebug($"---* Related recorded sequence found");
                    return seqInfo;
                }
            }
            LogUserDebug($"---* No recorded sequence matched");
            return null;
        }

        private static void ClearSeqRecordCache()
        {
            playerActionSeqs.Clear();
            skippedSeqs.Clear();
        }

        private static bool IsSeqExpired(ActionSeqInfo info)
            => info.ElapsedSeconds > SeqExpiry;

        private static void OnClassJobChangedClearCache(uint classJobId)
            => ClearSeqRecordCache();

        private static void OnTerritoryChangedClearCache(object? sender, ushort terr)
            => ClearSeqRecordCache();


        static ActionWatcher()
        {
            UseActionHook ??= Hook<UseActionDelegate>.FromAddress(
                ActionManagerHelper.FpUseAction, UseActionDetour);
            UseActionLocationHook ??= Hook<UseActionLocationDelegate>.FromAddress(
                ActionManagerHelper.FpUseActionLocation, UseActionLocationDetour);
            ReceiveActionEffectHook ??= Hook<ReceiveActionEffectDelegate>.FromAddress(
                SigScanner.ScanText("E8 ?? ?? ?? ?? 48 8B 8D F0 03 00 00"), 
                ReceiveActionEffectDetour);
            SendActionHook ??= Hook<SendActionDelegate>.FromAddress(
                SigScanner.ScanText("E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? F3 0F 10 3D ?? ?? ?? ?? 48 8D 4D BF"), 
                SendActionDetour);

            LogInformation("ActionWatcher init:\n" +
                $"\tUseActionHook @{UseActionHook?.Address ?? IntPtr.Zero:X}\n" +
                $"\tUseActionLoactionHook @{UseActionLocationHook?.Address ?? IntPtr.Zero:X}\n" +
                $"\tReceiveActionEffectHook @{ReceiveActionEffectHook?.Address ?? IntPtr.Zero:X}\n" +
                $"\tSendActionHook @{SendActionHook?.Address ?? IntPtr.Zero:X}");
        }

        public static void Enable()
        {
            UseActionHook?.Enable();
            UseActionLocationHook?.Enable();
            SendActionHook?.Enable();
            ReceiveActionEffectHook?.Enable();

            ClientState.TerritoryChanged += OnTerritoryChangedClearCache;
            ClassJobWatcher.ClassJobChanged += OnClassJobChangedClearCache;
        }

        public static void Disable()
        {
            UseActionHook?.Disable();
            UseActionLocationHook?.Disable();
            SendActionHook?.Disable();
            ReceiveActionEffectHook?.Disable();

            ClientState.TerritoryChanged -= OnTerritoryChangedClearCache;
            ClassJobWatcher.ClassJobChanged -= OnClassJobChangedClearCache;
        }

        public static void Dispose()
        {
            Disable();
            UseActionHook?.Dispose();
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
        // 0x14 Unk; but have some value keep accumulating here
        [FieldOffset(0x14)] public uint UnkObjectId;
        // 0x18 Corresponds exactly to the sequence of the action used;
        //      AA, pet's action effect etc. will be 0 here
        [FieldOffset(0x18)] public ushort Sequence;
        // 0x1A Seems related to SendAction's arg a5, but not always the same value
        [FieldOffset(0x1A)] public ushort Unk_1A;
        // rest??
    }
}
