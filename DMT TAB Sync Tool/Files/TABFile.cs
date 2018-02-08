using System;
using System.IO;
using System.Linq;
using NLog;
using TABSync.Properties;

namespace TABSync {
    abstract class TabFile {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly        string filePath;

        public TabFile(string FilePath) {
            filePath = FilePath;
        }

        public void ConvertToCSV(string outputFolder) {
            try {
                if (!Directory.Exists(outputFolder))
                    Directory.CreateDirectory(outputFolder);

                var baseFileName = Path.GetFileNameWithoutExtension(filePath);
                var tabPath      = Path.Combine(Settings.Default.TABPath, Path.ChangeExtension(baseFileName, "tab"));
                var csvPath      = Path.Combine(outputFolder, Path.ChangeExtension(baseFileName, "csv"));

                logger.Info($"Converting {tabPath} to {csvPath}...");

                var input = File.ReadAllLines(tabPath);
                var lines = input.Select(row => row.Split('\t'));
                lines = lines.Select(row => row.Select(field => field.EscapeCsvField('"')).ToArray());

                var csv = lines.Select(row => string.Join(",", row));

                File.WriteAllLines(csvPath, csv.ToArray());
            } catch (Exception ex) {
                logger.Error($"Failed to convert file. Error: {ex.Message}");
            }
        }
    }

    internal static class Extension {
        public static string EscapeCsvField(this string source, char escapeChar) {
            return string.Format("{0}{1}{0}", escapeChar, source);
        }
    }
}