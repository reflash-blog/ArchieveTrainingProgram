using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArchieveAlgorithms
{
    public class RleArchieveAlgorithm : IArchieveAlgorithm
    {
        public async Task<byte[]> Archieve(byte[] inputBytes)
        {
            return await Task.Run(() =>
            {
                byte[] resultBytes = null;
                var resultList = new List<byte>();
                var displacement = 0;
                for (var i = 0; i < inputBytes.Length - 1; i++)
                {
                    var prefixByte = new byte();
                    var subByteArrayLength = 1;
                    if (inputBytes[i] == inputBytes[i + 1])
                    {
                        prefixByte = (byte) (prefixByte | 0x80);
                        while (i < inputBytes.Length - 1 & subByteArrayLength < 128)
                        {
                            if (inputBytes[i] == inputBytes[i + 1])
                            {
                                subByteArrayLength++;
                                i++;
                            }
                            else
                            {
                                break;
                            }

                        }
                        prefixByte = (byte) (prefixByte | subByteArrayLength - 2);
                        resultList.Add(prefixByte);
                        resultList.Add(inputBytes[displacement]);
                        displacement += subByteArrayLength;
                        continue;
                    }
                    subByteArrayLength = 0;
                    prefixByte = (byte) (prefixByte | 0x00);
                    while (i < inputBytes.Length - 1 & subByteArrayLength < 128)
                    {
                        if (inputBytes[i] != inputBytes[i + 1])
                        {
                            subByteArrayLength++;
                            i++;
                        }
                        else
                        {
                            i--;
                            break;
                        }
                    }
                    prefixByte = (byte) (prefixByte | subByteArrayLength - 1);
                    resultList.Add(prefixByte);
                    for (var j = displacement; j < displacement + subByteArrayLength; j++)
                    {
                        resultList.Add(inputBytes[j]);
                    }
                    displacement += subByteArrayLength;
                }
                resultBytes = new byte[resultList.Count];
                for (var i = 0; i < resultList.Count; i++)
                    resultBytes[i] = resultList[i];

                return resultBytes;
            });

        }
    }
}
