using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onboarding.Models
{
    public class Chunk
    {
        public int StartByte { get; set; }
        public int EndByte { get; set; }
        public int Index { get; set; }
        private readonly string _filePath;

        public Chunk(string filePath, int startByte, int endByte, int index)
        {
            _filePath = filePath;
            StartByte = startByte;
            EndByte = endByte;
            Index = index;
        }

        public byte[] GetChunkData()
        {
            int length = EndByte - StartByte + 1;
            byte[] buffer = new byte[length];
            
            using (FileStream fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
            {
                fs.Position = StartByte;
                fs.Read(buffer, 0, length);
            }
            
            return buffer;
        }
    }
}
