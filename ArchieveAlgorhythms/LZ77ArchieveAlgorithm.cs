using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchieveAlgorithms
{
    public class LZ77ArchieveAlgorithm:IArchieveAlgorithm
    {
        private const int searchWindow = 4095;
        private const byte lookaheadWindow = 15;

        public async Task<byte[]> Archieve(byte[] inputBytes)
        {
            return await Task.Run(() =>
            {
                int position = 0;
                List<byte> tempInput = inputBytes.ToList();
                List<byte> output = new List<byte>();
                MemoryStream init = new MemoryStream();
                BinaryWriter inbw = new BinaryWriter(init);
                inbw.Write(((inputBytes.Length << 8) & 0xFFFFFF00) | 0x10);
                output.AddRange(init.ToArray());
                while (position < inputBytes.Length)
                {
                    byte decoder = 0;
                    List<byte> tempOutput = new List<byte>();
                    for (int i = 0; i < 8; ++i)
                    {
                        List<byte> eligible;
                        if (position < 255)
                        {
                            eligible = tempInput.GetRange(0, position);
                        }
                        else
                        {
                            eligible = tempInput.GetRange(Math.Abs(position - searchWindow), searchWindow);
                        }
                        if (!(position > inputBytes.Length - 8))
                        {
                            MemoryStream ms = new MemoryStream(eligible.ToArray());
                            List<byte> currentSequence = new List<byte>();
                            currentSequence.Add(inputBytes[position]);
                            int offset = 0;
                            int length = 0;
                            long tempoffset = FindPosition(ms, currentSequence.ToArray());
                            while ((tempoffset != -1) && (length < lookaheadWindow) && position < inputBytes.Length - 8)
                            {
                                offset = (int) tempoffset;
                                length = currentSequence.Count;
                                position++;
                                currentSequence.Add(inputBytes[position]);

                            }
                            if (length >= 3)
                            {
                                decoder = (byte) (decoder | (byte) (1 << i));
                                byte b1 = (byte) ((length << 4) | (offset >> 8));
                                byte b2 = (byte) (offset & 0xFF);
                                tempOutput.Add(b1);
                                tempOutput.Add(b2);
                            }
                            else
                            {
                                tempOutput.Add(inputBytes[position]);
                                position++;
                            }
                        }
                        else
                        {
                            if (position < inputBytes.Length)
                            {
                                tempOutput.Add(inputBytes[position]);
                                position++;
                            }
                            else
                            {
                                tempOutput.Add(0xFF);
                            }
                        }
                    }
                    output.Add(decoder);
                    output.AddRange(tempOutput.ToArray());
                }
                return output.ToArray();
            });
        }

        private long FindPosition(Stream stream, byte[] byteSequence)
        {
            if (byteSequence.Length > stream.Length)
                return -1;

            byte[] buffer = new byte[byteSequence.Length];

            using (BufferedStream bufStream = new BufferedStream(stream, byteSequence.Length))
            {
                int i;
                while ((i = bufStream.Read(buffer, 0, byteSequence.Length)) == byteSequence.Length)
                {
                    if (byteSequence.SequenceEqual(buffer))
                        return bufStream.Position - byteSequence.Length;
                    else
                        bufStream.Position -= byteSequence.Length - PadLeftSequence(buffer, byteSequence);
                }
            }

            return -1;
        }

        private int PadLeftSequence(byte[] bytes, byte[] seqBytes)
        {
            int i = 1;
            while (i < bytes.Length)
            {
                int n = bytes.Length - i;
                byte[] aux1 = new byte[n];
                byte[] aux2 = new byte[n];
                Array.Copy(bytes, i, aux1, 0, n);
                Array.Copy(seqBytes, aux2, n);
                if (aux1.SequenceEqual(aux2))
                    return i;
                i++;
            }
            return i;
        }
    }
}
