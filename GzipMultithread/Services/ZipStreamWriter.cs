using System.IO;

namespace GzipMultithread.Services
{
    public class ZipStreamWriter : StreamWriter
    {
        public ZipStreamWriter(string destinationPath) : base(destinationPath)
        {
        }

        public override void Write()
        {
            using (var fs = new FileStream($"{DestinationPath}.zip", FileMode.Append))
            {
                WriteStream(fs);
                // using (var archive = new ZipArchive(fs, ZipArchiveMode.Create))
                // {
                //     var entry = archive.CreateEntry(SourceFileName, CompressionLevel.NoCompression);
                //     using (var entryStream = entry.Open())
                //     {
                //         WriteStream(entryStream);
                //     }
                // }
            }
        }
    }
}