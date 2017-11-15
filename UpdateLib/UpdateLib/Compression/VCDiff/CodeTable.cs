using MatthiWare.UpdateLib.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatthiWare.UpdateLib.Compression.VCDiff
{
    internal class CodeTable
    {
        internal static CodeTable Default = BuildDefaultCodeTable();
        Instruction[,] m_instructions = new Instruction[256, 2];

        internal CodeTable(Instruction[,] instructions)
        {
            if (instructions == null)
                throw new ArgumentNullException(nameof(instructions));
            if (instructions.Rank != 2)
                throw new ArgumentException("Array must have a rank of 2", nameof(instructions));
            if (instructions.GetLength(0) != 256)
                throw new ArgumentException("Array must have a outer length of 256", nameof(instructions));
            if (instructions.GetLength(1) != 2)
                throw new ArgumentException("Array must have a innter length of 2", nameof(instructions));

            Array.Copy(instructions, 0, m_instructions, 0, 512);
        }

        internal Instruction this[int x, int y]
        {
            get
            {
                return m_instructions[x, y];
            }
        }

        internal static CodeTable BuildDefaultCodeTable()
        {
            // default are NoOps with size and mode 0.
            Instruction[,] instructions = new Instruction[256, 2];
            instructions[0, 0] = new Instruction(InstructionType.Run, 0, 0);

            for (byte i = 0; i < 18; i++)
                instructions[i + 1, 0] = new Instruction(InstructionType.Add, i, 0);

            int index = 19;

            // instructions 19-162
            for (byte mode = 0; mode < 9; mode++)
            {
                instructions[index++, 0] = new Instruction(InstructionType.Copy, 0, mode);
                for (byte size = 4; size < 19; size++)
                    instructions[index++, 0] = new Instruction(InstructionType.Copy, size, mode);
            }

            // instructions 163-234
            for (byte mode = 0; mode < 6; mode++)
                for (byte addSize = 1; addSize < 5; addSize++)
                    for (byte copySize = 4; copySize < 7; copySize++)
                    {
                        instructions[index, 0] = new Instruction(InstructionType.Add, addSize, 0);
                        instructions[index++, 0] = new Instruction(InstructionType.Copy, copySize, mode);
                    }

            // instructions 235-246
            for (byte mode = 6; mode < 9; mode++)
                for (byte addSize = 1; addSize < 5; addSize++)
                {
                    instructions[index, 0] = new Instruction(InstructionType.Add, addSize, 0);
                    instructions[index++, 1] = new Instruction(InstructionType.Copy, 4, mode);
                }

            for (byte mode = 0; mode < 9; mode++)
            {
                instructions[index, 0] = new Instruction(InstructionType.Copy, 4, mode);
                instructions[index++, 1] = new Instruction(InstructionType.Add, 1, 0);
            }

            return new CodeTable(instructions);
        }

        internal byte[] GetBytes()
        {
            byte[] ret = new byte[1536];

            for (int i = 0; i < 256; i++)
            {
                ret[i] = (byte)m_instructions[i, 0].Type;
                ret[i + 256] = (byte)m_instructions[i, 1].Type;
                ret[i + 512] = m_instructions[i, 0].Size;
                ret[i + 768] = m_instructions[i, 1].Size;
                ret[i + 1024] = m_instructions[i, 0].Size;
                ret[i + 1028] = m_instructions[i, 1].Size;
            }

            return ret;
        }
    }
}
