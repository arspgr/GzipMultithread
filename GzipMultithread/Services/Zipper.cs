using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using GzipMultithread.Models;
using GzipMultithread.Settings;

namespace GzipMultithread.Services
{
    public class Zipper
    {
        public event ProgramSettings.FilePartBufferHandler NotifyPartBufferProcessed;

        public void ProcessParts(IEnumerable<FilePart> parts)
        {
            var result = new ConcurrentBag<FilePart>();
            
            Parallel.ForEach(parts, new ParallelOptions
            {
                MaxDegreeOfParallelism = ProgramSettings.MaxThreadCount
            }, part =>
            {
                result.Add(ProcessPart(part));
            });
            
            NotifyPartBufferProcessed?.Invoke(result.ToList());
        }
        
        protected virtual FilePart ProcessPart(FilePart filePart)
        {
            if (filePart == null)
            {
                return null;
            }

            using (var memoryStream = new MemoryStream())
            {
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress))
                {
                    gZipStream.Write(filePart.Bytes, 0, filePart.Bytes.Length);
                }

                var compressedBytes = memoryStream.ToArray();
                BitConverter.GetBytes(compressedBytes.Length).CopyTo(compressedBytes, 4);
                
                return new FilePart
                {
                    Index = filePart.Index,
                    Bytes = compressedBytes
                };
            }
        }
    }
}