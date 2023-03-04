using System.Runtime.InteropServices;

namespace ActionEffectRange.Utils
{
    internal static class Interop
    {
        public static float MarshalFloat(IntPtr p)
            => BitConverter.ToSingle(BitConverter.GetBytes(Marshal.ReadInt32(p)));
    }
}
