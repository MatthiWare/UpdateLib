using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MatthiWare.UpdateLib.Utils;
using MatthiWare.UpdateLib.Common;

namespace MatthiWare.UpdateLib.Compression.VCDiff
{
    public sealed class VCDiffDecoder
    {

        private Stream m_original, m_delta, m_output;

        private CodeTable m_codeTable = CodeTable.Default;

        private AddressCache m_cache = new AddressCache(4, 3);

        private VCDiffDecoder(Stream original, Stream delta, Stream output)
        {
            m_original = original;
            m_delta = delta;
            m_output = output;
        }

        public static void Decode(Stream original, Stream delta, Stream output)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));
            if (delta == null) throw new ArgumentNullException(nameof(delta));
            if (output == null) throw new ArgumentNullException(nameof(output));

            if (!original.CanRead || !original.CanSeek)
                throw new ArgumentException("Must be able to read and seek in stream", nameof(original));

            if (!delta.CanRead)
                throw new ArgumentException("Must be able to read in stream", nameof(delta));

            if (!output.CanWrite || !output.CanSeek || !output.CanRead)
                throw new ArgumentException("Must be able to read, seek and write in stream", nameof(output));

            VCDiffDecoder decoder = new VCDiffDecoder(original, delta, output);
            decoder.Decode();
        }

        private void Decode()
        {
            ReadHeader();
        }

        private void ReadHeader()
        {
            byte[] header = m_delta.CheckedReadBytes(4);
            if (header[0] != 0xd6 ||
                header[1] != 0xc3 ||
                header[2] != 0xc4)
                throw new VCDiffFormatException("Invalid header in delta stream");

            if (header[3] != 0)
                throw new VCDiffFormatException("Only VCDiff Version 0 is supported");

            byte headerIndicator = m_delta.CheckedReadByte();

            if ((headerIndicator & 1) != 0)
                throw new VCDiffFormatException("Secondary compressors are not supported");

            if ((headerIndicator & 0xf8) != 0)
                throw new VCDiffFormatException("Invalid header indicator, bits 3-7 not all zero");
        }

        private bool DecodeWindow()
        {
            int windowIndicator = m_delta.ReadByte();

            if (windowIndicator == -1)
                return false;

            Stream sourceStream;
            int sourceStreamPostReadSeek = -1;
            windowIndicator &= 0xfb;

            switch (windowIndicator & 3)
            {
                case 0:
                    sourceStream = null;
                    break;
                case 1:
                    sourceStream = m_original;
                    break;
                case 2:
                    sourceStream = m_output;
                    sourceStreamPostReadSeek = (int)m_output.Position;
                    break;
                default:
                    throw new VCDiffFormatException("Invalid window indicator");
            }


            int sourceLength = m_delta.ReadBigEndian7BitEncodedInt();
            int sourcePosition = m_delta.ReadBigEndian7BitEncodedInt();

            sourceStream.Position = sourcePosition;
            byte[] sourceData = sourceStream.CheckedReadBytes(sourceLength);
            if (sourceStreamPostReadSeek != -1)
                sourceStream.Position = sourceStreamPostReadSeek;

            m_delta.ReadBigEndian7BitEncodedInt();

            int targetLength = m_delta.ReadBigEndian7BitEncodedInt();
            byte[] targetData = new byte[targetLength];
            MemoryStream targetDataStream = new MemoryStream(targetData, true);

            if (m_delta.CheckedReadByte() != 0)
                throw new VCDiffFormatException("Unable to handle compressed delta sections");

            int addRunDataLength = m_delta.ReadBigEndian7BitEncodedInt();
            int instructionsLength = m_delta.ReadBigEndian7BitEncodedInt();
            int addressesLength = m_delta.ReadBigEndian7BitEncodedInt();

            byte[] addRunData = m_delta.CheckedReadBytes(addRunDataLength);
            byte[] instructions = m_delta.CheckedReadBytes(instructionsLength);
            byte[] addresses = m_delta.CheckedReadBytes(addressesLength);

            int addRunDataIndex = 0;
            MemoryStream instructionStream = new MemoryStream(instructions, false);

            m_cache.Reset(addresses);

            while (true)
            {
                int instructionIndex = instructionStream.ReadByte();
                if (instructionIndex == -1)
                    break;

                for (int i = 0; i < 2; i++)
                {
                    Instruction instruction = m_codeTable[instructionIndex, i];
                    int size = instruction.Size;

                    if (size == 0 && instruction.Type != InstructionType.NoOp)
                        size = instructionStream.ReadBigEndian7BitEncodedInt();

                    switch (instruction.Type)
                    {
                        case InstructionType.NoOp:
                            break;
                        case InstructionType.Add:
                            targetDataStream.Write(addRunData, addRunDataIndex, size);
                            addRunDataIndex += size;
                            break;
                        case InstructionType.Copy:
                            int addr = m_cache.DecodeAddress((int)targetDataStream.Position + sourceLength, instruction.Mode);
                            if (addr < sourceData.Length)
                                targetDataStream.Write(sourceData, addr, size);
                            else
                            {
                                addr -= sourceLength;
                                if (addr + size < targetDataStream.Position)
                                    targetDataStream.Write(targetData, addr, size);
                                else
                                    for (int j = 0; j < size; j++)
                                        targetDataStream.WriteByte(targetData[addr++]);
                            }
                            break;
                        case InstructionType.Run:
                            byte data = addRunData[addRunDataIndex++];
                            for (int j = 0; j < size; j++)
                                targetDataStream.WriteByte(data);
                            break;
                        default:
                            throw new VCDiffFormatException("Invalid instruction type found");
                    }
                }

                m_output.Write(targetData, 0, targetLength);
            }

            return true;
        }
    }
}
