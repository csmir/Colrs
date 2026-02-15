using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Colrs
{
    public readonly partial struct Colr
    {
        [StructLayout(LayoutKind.Explicit)]
        private struct SystemColor
        {
            [FieldOffset(0)]
            public nint Name;

            [FieldOffset(8)]
            public long Value;

            [FieldOffset(16)]
            public short KnownColor;

            [FieldOffset(18)]
            public short State;
        }

        /// <summary>
        ///     Creates a <see cref="Color"/> from the current <see cref="Colr"/> instance.
        /// </summary>
        /// <returns>A new <see cref="Color"/> created from the sRGB value of this <see cref="Colr"/>.</returns>
        public unsafe Color ToSystem()
        {
            var systemColor = new SystemColor
            {
                Value = Value,
                State = 0x0002 // Refer to System.Drawing.Color.StateARGBValueValid.
            };

            return Unsafe.As<SystemColor, Color>(ref systemColor);
        }
    }
}
