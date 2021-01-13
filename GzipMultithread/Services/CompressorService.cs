using System;
using System.Collections.Generic;
using System.Threading;
using GzipMultithread.Extensions;
using GzipMultithread.Models;

namespace GzipMultithread.Services
{
    public class CompressorService
    {
        public CompressorService(Zipper zipper, FileReader reader, StreamWriter writer)
        {
            _zipper = zipper;
            _reader = reader;
            _writer = writer;
        }

        private readonly Zipper _zipper;
        private readonly FileReader _reader;
        private readonly StreamWriter _writer;

        private readonly AutoResetEvent _writerCompletedEvent = new AutoResetEvent(false);

        public void Start()
        {
            _writer.NotifyPartsWrote += OnPartsWrote;
            _writer.NotifyCompleted += OnWriterCompleted;
            _reader.NotifyPartBufferRead += OnPartBufferRead;
            _reader.NotifyCompleted += OnReadComplete;
            _zipper.NotifyPartBufferProcessed += OnPartBufferProcessed;

            var readThread = new Thread(_reader.Read);
            readThread.Start();

            var writeThread = new Thread(_writer.Write);
            writeThread.Start();

            _writerCompletedEvent.WaitOne();
        }

        private void OnPartBufferRead(List<FilePart> parts)
        {
            _zipper.ProcessParts(parts);
        }

        private void OnPartBufferProcessed(List<FilePart> parts)
        {
            Console.WriteLine($"Parts {parts.PartIds()} processed");
            _writer.SetPartsForWrite(parts);
        }

        private void OnPartsWrote()
        {
            _reader.ContinueWork();
        }

        private void OnReadComplete()
        {
            _writer.IsCompleted = true;
        }

        private void OnWriterCompleted()
        {
            Console.WriteLine("File write completed");
            _writerCompletedEvent.Set();
        }
    }
}