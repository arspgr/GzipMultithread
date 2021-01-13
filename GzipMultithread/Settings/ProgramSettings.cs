using System;
using System.Collections.Generic;
using GzipMultithread.Models;

namespace GzipMultithread.Settings
{
    public static class ProgramSettings
    {
        public static int MaxThreadCount { get; } = Environment.ProcessorCount;
        public static int MaxFilePartBufferCount { get; } = MaxThreadCount * 2;
        public static int MaxBytes { get; } = 10000000;
        public delegate void FilePartBufferHandler(List<FilePart> part);
        
        public static string SourceFilePath { get; set; }
        public static string DestinationFilePath { get; set; }
    }
}