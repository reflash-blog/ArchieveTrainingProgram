using ArchieveAlgorithms;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArchieveAlgorithmUnitTest
{
    [TestClass]
    public class RleArchieveAlgorithmTest
    {
        [TestMethod]
        public async void RleArchieveAlgorithmMethodTest()
        {
            var archivator = new RleArchieveAlgorithm();
            var results = await archivator.Archieve(new byte[]
            {
                0, 0, 0, 0, 0, 0, 4, 2, 0, 4, 4, 4, 4, 4, 4, 4,
                80, 80, 80, 80, 0, 2, 2, 2, 2, 255, 255, 255, 255, 255, 0, 0
            }
                );
            Assert.AreEqual(18, results);
            var expectedResult = new byte[]
            {
                0x84, 0x00, 0x02, 0x04, 0x02, 0x00, 0x85, 0x04, 0x82, 0x50, 0x00, 0x00, 0x82, 0x02, 0x83, 0xFF,
                0x80, 0x00
            };
            Assert.AreEqual(expectedResult[0], results[0]);
        }
    }
}
