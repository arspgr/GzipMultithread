using System.Collections.Generic;
using System.IO;
using System.Linq;
using GzipMultithread.Models;

namespace GzipMultithread.Services
{
    public abstract class StreamWriter
    {
        protected StreamWriter(string destinationPath)
        {
            DestinationPath = destinationPath;
        }

        protected readonly string DestinationPath;


        private List<FilePart> _parts = new List<FilePart>();

        public delegate void WriteHandler();

        public event WriteHandler NotifyPartsWrote;
        public event WriteHandler NotifyCompleted;

        public bool IsCompleted { get; set; }

        protected void WriteStream(Stream stream)
        {
            while (true)
            {
                WriteParts(stream);

                if (!IsCompleted || _parts != null) continue;

                NotifyCompleted?.Invoke();
                break;
            }
        }

        public abstract void Write();

        private void WriteParts(Stream stream)
        {
            if (_parts == null || !_parts.Any())
            {
                return;
            }

            foreach (var filePart in _parts.OrderBy(p => p.Index))
            {
                stream.Write(filePart.Bytes, 0, filePart.Bytes.Length);
            }

            NotifyPartsWrote?.Invoke();
            _parts = null;
        }

        public void SetPartsForWrite(List<FilePart> parts)
        {
            if (_parts != null && _parts.Any())
            {
                return;
            }

            _parts = parts;
        }
    }
}