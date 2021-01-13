using System.Collections.Generic;
using System.Linq;
using GzipMultithread.Models;

namespace GzipMultithread.Extensions
{
    public static class FilePartExtension
    {
        public static string PartIds(this IEnumerable<FilePart> parts)
        {
            return string.Join(",",parts.Select(p => p.Index));
        }
    }
}