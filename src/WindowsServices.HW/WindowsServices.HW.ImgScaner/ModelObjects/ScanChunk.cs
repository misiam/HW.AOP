using System;
using System.Collections.Generic;

namespace WindowsServices.HW.ImgScanner.ModelObjects
{
    public class ScanChunk
    {
        public IList<string> ChunkFiles { get; set; } = new List<string>();
        public string Name { get; set; }
        public DateTime Created { get; set; } = new DateTime();

    }
}
