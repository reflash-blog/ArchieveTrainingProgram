using System.Collections;
using System.Threading.Tasks;

namespace ArchieveAlgorithms.HuffmanAlgorithm
{
    public class HuffmanArchieveAlgorithm:IArchieveAlgorithm
    {
        public async Task<byte[]> Archieve(byte[] inputBytes)
        {
            return await Task.Run(() =>
            {
                var input = System.Text.Encoding.UTF8.GetString(inputBytes);
                var huffmanTree = new HuffmanTree();

                // Build the Huffman tree
                huffmanTree.Build(input);

                // Encode
                var encoded = huffmanTree.Encode(input);
                return BitArrayToByteArray(encoded);
            });
        }

        private byte[] BitArrayToByteArray(BitArray bits)
        {
            var ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret;
        }
    }
}
