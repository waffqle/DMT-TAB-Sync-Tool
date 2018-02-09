using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;

namespace TABSync.Imports {
    internal class Import {
        protected Import(string path) {
            TabPath = path;

            // What country is this from. (We'll have to set currency and such.)
            CountryOfOrigin = getSourceCountry(path);

            // Read in the tab lines and pop each line into an array of fields.
            rawLines = File.ReadLines(TabPath).Select(l => l.Split('\t')).ToList();

            logger.Debug($"Read {rawLines.Count} items from {Path.GetFileName(TabPath)}.");
        }

        protected readonly        Country        CountryOfOrigin;
        protected readonly        List<string[]> rawLines;
        private readonly          string         TabPath;
        protected                 List<string>   Headers = new List<string>();
        protected                 List<string>   Lines   = new List<string>();
        protected static readonly Logger         logger  = LogManager.GetCurrentClassLogger();

        private Country getSourceCountry(string path) {
            var fileName = Path.GetFileNameWithoutExtension(path);
            if (fileName.Contains("_CA_"))
                return Country.CA;
            if (fileName.Contains("_USA_"))
                return Country.US;

            logger.Warn("Couldn't determine country. Defaulting to Canada.");
            return Country.CA;
        }

        public void ExportDMTFile(string outputFolder) {
            /*
             * Use the same name as the source file. 
             * Just change the extension and location.
             */
            var csvPath = CalculateCSVPath(outputFolder);
            logger.Info($"Writing {csvPath}");

            // Create CSV with headers
            var csv = new List<string> {
                string.Join(",", Headers.Select(h => h.EscapeCsvField('"')))
            };

            // Add the lines
            csv.AddRange(Lines);

            // Dump it to disk.
            File.WriteAllLines(csvPath, csv.ToArray());
        }

        private string CalculateCSVPath(string outputFolder) {
            var baseName = Path.GetFileNameWithoutExtension(TabPath);
            var csvName  = Path.ChangeExtension(baseName, "csv");
            var csvPath  = Path.Combine(outputFolder, csvName);
            return csvPath;
        }

        protected string GetCurrencyCode(Country country) {
            switch (country) {
                case Country.CA: return "CAD";
                case Country.US: return "USD";
                case Country.EU: return "ERU";
                default:         throw new ArgumentOutOfRangeException(nameof(country), country, null);
            }
        }

        protected enum Country {
            CA,
            EU,
            US
        }
    }

    internal static class Extension {
        public static string EscapeCsvField(this string source, char escapeChar) {
            return string.Format("{0}{1}{0}", escapeChar, source);
        }
    }
}