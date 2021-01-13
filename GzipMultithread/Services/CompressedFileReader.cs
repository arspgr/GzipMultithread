using System;
using System.IO;
using GzipMultithread.Models;

namespace GzipMultithread.Services
{
    public class CompressedFileReader : FileReader
    {
        public CompressedFileReader(string filePath) : base(filePath)
        {
        }

        protected override FilePart GetFilePart(Stream stream, int index)
        {
            var lengthBuffer = new byte[8];
            stream.Read(lengthBuffer, 0, lengthBuffer.Length);
            var blockLength = BitConverter.ToInt32(lengthBuffer, 4);
            var compressedBytes = new byte[blockLength];
            lengthBuffer.CopyTo(compressedBytes, 0);

            stream.Read(compressedBytes, 8, blockLength - 8);
            var dataSize = BitConverter.ToInt32(compressedBytes, blockLength - 4);

            return new CompressedFilePart
            {
                Index = index,
                Bytes = compressedBytes,
                OriginalBufferSize = dataSize
            };
        }
    }
}