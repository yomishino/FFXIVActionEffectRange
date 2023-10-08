using FFXIVClientStructs.FFXIV.Client.Game;
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
        public static float CastTargetPosX => actionMgrPtr != IntPtr.Zero
            ? Interop.MarshalFloat(actionMgrPtr + 0x40) : 0f;
        public static float CastTargetPosY => actionMgrPtr != IntPtr.Zero
            ? Interop.MarshalFloat(actionMgrPtr + 0x44) : 0f;
        public static float CastTargetPosZ => actionMgrPtr != IntPtr.Zero
            ? Interop.MarshalFloat(actionMgrPtr + 0x48) : 0f;
        // The player rotation when casting
        public static float CastRotation => actionMgrPtr != IntPtr.Zero
            ? Interop.MarshalFloat(actionMgrPtr + 0x50) : 0f;
        
        static ActionManagerHelper()
        {
            actionMgrPtr = (IntPtr)ActionManager.Instance();
            if (actionMgrPtr == IntPtr.Zero)
                PluginLog.Warning("Ptr to ActionManager is 0");
        }
    }
}
