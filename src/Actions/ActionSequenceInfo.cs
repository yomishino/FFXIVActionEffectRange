using System;
using System.Collections.Generic;
using System.Numerics;

namespace ActionEffectRange.Actions
{
    public class ActionSequenceInfo
    {
        public readonly EffectRangeData EffectRangeData = null!;
        public readonly Vector3 OriginPosition;
        public readonly Vector3 TargetPosition;
        public readonly float ActorRotation;
        public readonly bool IsPetAction;    // incl. pet-like such as bunshin's
        public uint ActionId => EffectRangeData.ActionId;

        public ActionSequenceInfo(EffectRangeData effectRangeData, Vector3 originPos, Vector3 targetPos, float actorRotation, bool isPetAction = false)
        {
            EffectRangeData = effectRangeData;
            OriginPosition = originPos;
            TargetPosition = targetPos;
            ActorRotation = actorRotation;
            IsPetAction = isPetAction;
        }
    }


    public class ActionSequenceInfoSet : HashSet<ActionSequenceInfo>
    {
        public readonly ushort ActionSequence;

        public ActionSequenceInfoSet(ushort sequence) : base() => ActionSequence = sequence;

    }
}
