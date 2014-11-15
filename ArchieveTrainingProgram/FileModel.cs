using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchieveTrainingProgram
{
    public class FileModel
    {
        public string Path { get; set; }
        public byte[] FileBytes { get; set; }
        public int Capacity { get { return FileBytes.Length; } }
    }
}
