using System.Threading.Tasks;

namespace ArchieveAlgorithms
{
    public interface IArchieveAlgorithm
    {
        Task<byte[]> Archieve(byte[] inputBytes);
    }
}
