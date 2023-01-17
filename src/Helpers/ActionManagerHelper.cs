using FFXIVClientStructs.FFXIV.Client.Game;
using System;
using System.Runtime.InteropServices;

namespace ActionEffectRange.Helpers
{
    internal unsafe static class ActionManagerHelper
    {
        private static readonly IntPtr actionMgrPtr;

        internal static IntPtr FpUseAction => 
            (IntPtr)ActionManager.MemberFunctionPointers.UseAction;
        internal static IntPtr FpUseActionLocation => 
            (IntPtr)ActionManager.MemberFunctionPointers.UseActionLocation;

        public static ushort CurrentSeq => actionMgrPtr != IntPtr.Zero
            ? (ushort)Marshal.ReadInt16(actionMgrPtr + 0x110) : (ushort)0;
        public static ushort LastRecievedSeq => actionMgrPtr != IntPtr.Zero
            ? (ushort)Marshal.ReadInt16(actionMgrPtr + 0x112) : (ushort)0;

        public static bool IsCasting => actionMgrPtr != IntPtr.Zero
            && Marshal.ReadByte(actionMgrPtr + 0x28) != 0; 
        public static uint CastingActionId => actionMgrPtr != IntPtr.Zero
            ? (uint)Marshal.ReadInt32(actionMgrPtr + 0x24) : 0u;
        public static uint CastTargetObjectId => actionMgrPtr != IntPtr.Zero
            ? (uint)Marshal.ReadInt32(actionMgrPtr + 0x38) : 0u;

        static ActionManagerHelper()
        {
            actionMgrPtr = (IntPtr)ActionManager.Instance();
        }
    }
}
