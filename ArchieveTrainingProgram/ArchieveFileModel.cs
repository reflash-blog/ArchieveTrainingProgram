using ArchieveAlgorithms;

namespace ArchieveTrainingProgram
{
    public class ArchieveFileModel
    {
        public string Name { get; set; }
        public IArchieveAlgorithm ArchieveAlgorithm { get; set; }
        public string AlgorithmNotes { get; set; }
    }
}
