using System;
using System.IO;
using System.Linq;
using GzipMultithread.Settings;

namespace GzipMultithread.Extensions
{
    public static class StringArrayExtension
    {
        public static void ValidateArguments(this string[] args)
        {
            if (args == null || !args.Any())
            {
                throw new Exception("Необходимо передать аргументы: compress / decompress, имя исходного файла, имя результирующего файла");
            }

            if (!Enum.TryParse<ProgramMode>(args[0], true, out var mode))
            {
                throw new Exception($"Первым аргументом должно быть: {ProgramMode.Compress} или {ProgramMode.Decompress}");
            }

            if (string.IsNullOrEmpty(args[1]))
            {
                throw new Exception("Не задано имя исходного файла");
            }
            
            if (string.IsNullOrEmpty(args[2]))
            {
                throw new Exception("Не задано имя результирующего файла");
            }
            
            if (!File.Exists(args[1]))
            {
                throw new Exception($"Файла {args[1]} не существует");
            }
        }
    }
}