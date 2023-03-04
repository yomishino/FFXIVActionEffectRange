namespace ActionEffectRange.Actions
{
    public class ActionSeqInfo
    {
        public readonly uint ActionId;
        public readonly SeqSnapshot SeqSnapshot = null!;
        public readonly bool IsPetAction; // incl. pet-like such as bunshin's
        public uint Seq => SeqSnapshot.Seq;
        public DateTime SnapshotTime => SeqSnapshot.CreatedTime;
        public double ElapsedSeconds => SeqSnapshot.ElapsedSeconds;

        public ActionSeqInfo(uint actionId, 
            SeqSnapshot snapshot, bool isPetAction = false)
        {
            ActionId = actionId;
            SeqSnapshot = snapshot;
            IsPetAction = isPetAction;
        }
    }
}
