using MatthiWare.UpdateLib.Common;

namespace MatthiWare.UpdateLib.Compression.VCDiff
{
    internal struct Instruction
    {
        public readonly InstructionType Type;

        public readonly byte Size;

        public readonly byte Mode;

        internal Instruction(InstructionType type, byte size, byte mode)
        {
            Type = type;
            Size = size;
            Mode = mode;
        }
    }
}
