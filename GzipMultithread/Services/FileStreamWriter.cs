using System.IO;

namespace GzipMultithread.Services
{
    public class FileStreamWriter : StreamWriter
    {
        public FileStreamWriter(string destinationPath) : base(destinationPath)
        {
        }

        public override void Write()
        {
            using (var fs = new FileStream(DestinationPath, FileMode.Append))
            {
                WriteStream(fs);
            }
        }
    }
}