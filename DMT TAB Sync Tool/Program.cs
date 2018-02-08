using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using TABSync.Files;
using TABSync.Properties;

namespace TABSync {
    internal class Program {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private static void Main(string[] args) {
            /*
             * Collect those tab files and convert to CSV.
             * DMT likes CSV files.
             */
            foreach (var fileType in Enum.GetValues(typeof(FileType)).Cast<FileType>())
                ConvertTabToCsv(fileType);
        }

        private static void ConvertTabToCsv(FileType fileType) {
            var tabPaths  = GetFiles(Settings.Default.TABPath, GetFileRetrievalFilter(fileType));
            var tabFiles1 = tabPaths.Select(tp => new OrderHead(tp)).ToList();

            logger.Info($"Found {tabFiles1.Count} files.");

            var tabFiles = tabFiles1;
            foreach (var tf in tabFiles)
                tf.ConvertToCSV(Path.Combine(Settings.Default.CSVPath, GetDestinationFolder(fileType)));
        }

        private static IEnumerable<string> GetFiles(string path, string filter) {
            var files = Directory.EnumerateFiles(path, filter);
            return files;
        }

        private static string GetFileRetrievalFilter(FileType fileType) {
            switch (fileType) {
                case FileType.OrderHead: return "orders*.tab";
                case FileType.OrderLine: return "lines*.tab";
                default:                 throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null);
            }
        }

        private static string GetDestinationFolder(FileType fileType) {
            switch (fileType) {
                case FileType.OrderHead: return "OrderHeads";
                case FileType.OrderLine: return "OrderLines";
                default:                 throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null);
            }
        }

        private enum FileType {
            OrderHead,
            OrderLine
        }
    }
}