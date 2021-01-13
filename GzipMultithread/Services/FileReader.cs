using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using GzipMultithread.Models;
using GzipMultithread.Settings;

namespace GzipMultithread.Services
{
    public class FileReader
    {
        public FileReader(string filePath)
        {
            _filePath = filePath;
        }

        private readonly string _filePath;

        public event ProgramSettings.FilePartBufferHandler NotifyPartBufferRead;

        public delegate void CompletedHandler();

        public event CompletedHandler NotifyCompleted;

        private readonly EventWaitHandle _waitHandle = new AutoResetEvent(false);

        public void Read()
        {
            try
            {
                var filePartBuffer = new List<FilePart>();
                using (var fs = new FileStream(_filePath, FileMode.Open))
                {
                    var index = 0;
                    while (fs.Position < fs.Length)
                    {
                        filePartBuffer.Add(GetFilePart(fs, index));

                        index++;

                        if (filePartBuffer.Count >= ProgramSettings.MaxFilePartBufferCount)
                        {
                            filePartBuffer = NotifyPartsReadAndWait(filePartBuffer);
                        }
                    }
                }

                if (filePartBuffer.Any())
                {
                    NotifyPartsReadAndWait(filePartBuffer);
                }

                NotifyCompleted?.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void ContinueWork()
        {
            _waitHandle.Set();
        }

        private List<FilePart> NotifyPartsReadAndWait(List<FilePart> filePartBuffer)
        {
            NotifyPartBufferRead?.Invoke(filePartBuffer);
            filePartBuffer = new List<FilePart>();
            _waitHandle.WaitOne();
            return filePartBuffer;
        }

        protected virtual FilePart GetFilePart(Stream stream, int index)
        {
            var isLastBuffer = stream.Length - stream.Position <= ProgramSettings.MaxBytes;
            var bytesCount = isLastBuffer ? (int) (stream.Length - stream.Position) : ProgramSettings.MaxBytes;

            var buffer = new byte[bytesCount];
            stream.Read(buffer, 0, bytesCount);

            return new FilePart
            {
                Index = index,
                Bytes = buffer
            };
        }
    }
}