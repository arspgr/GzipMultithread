using System;
using System.IO;
using System.IO.Compression;
using GzipMultithread.Models;

namespace GzipMultithread.Services
{
    public class Unzipper : Zipper
    {
        protected override FilePart ProcessPart(FilePart filePart)
        {
            try
            {
                var compressedFilePart = (CompressedFilePart) filePart;
                using (var ms = new MemoryStream(compressedFilePart.Bytes))
                {
                    using (var gz = new GZipStream(ms, CompressionMode.Decompress))
                    {
                        var result = new byte[compressedFilePart.OriginalBufferSize];
                        gz.Read(result, 0, result.Length);

                        return new FilePart
                        {
                            Index = filePart.Index,
                            Bytes = result
                        };
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}