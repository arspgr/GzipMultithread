using System;
using GzipMultithread.Extensions;
using GzipMultithread.Services;
using GzipMultithread.Settings;

namespace GzipMultithread
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

            args.ValidateArguments();

            ProgramSettings.SourceFilePath = args[1];
            ProgramSettings.DestinationFilePath = args[2];

            Enum.TryParse<ProgramMode>(args[0], true, out var mode);
            CompressorService service;
            switch (mode)
            {
                case ProgramMode.Compress:
                    Console.WriteLine($"{ProgramMode.Compress} started");
                    service = new CompressorService(new Zipper(),
                        new FileReader(ProgramSettings.SourceFilePath),
                        new ZipStreamWriter(ProgramSettings.DestinationFilePath));
                    break;
                case ProgramMode.Decompress:
                    Console.WriteLine($"{ProgramMode.Decompress} started");
                    service = new CompressorService(new Unzipper(),
                        new CompressedFileReader(ProgramSettings.SourceFilePath),
                        new FileStreamWriter(ProgramSettings.DestinationFilePath));
                    break;
                default:
                    throw new Exception($"Неожиданный режим: {mode}");
            }

            service.Start();
        }

        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception) e.ExceptionObject;
            Console.WriteLine(ex.Message);
            Console.WriteLine("Press Enter to continue");
            Console.ReadLine();
            Environment.Exit(1);
        }
    }
}