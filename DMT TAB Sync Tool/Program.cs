using System;
using System.IO;
using NLog;
using TABSync.Imports;
using TABSync.Properties;

namespace TABSync {
    internal class Program {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private static void Main(string[] args) {
            /*
             * Convert the tab files into proper DMT templates.
             */
            foreach (var tabFile in Directory.GetFiles(Settings.Default.TABPath))
                RouteFile(tabFile);

            /*
             * Feed the converted templates to DMT
             */
        }

        private static void RouteFile(string tabFile) {
            logger.Info($"Processing: {Path.GetFileName(tabFile)}");

            var fileType = TabHelper.GetFileType(tabFile);
            switch (fileType) {
                case TabHelper.DataType.OrderHead: {
                    var orderHead = new OrderHead(tabFile);
                    orderHead.ExportDMTFile(Settings.Default.CSVPath);
                    break;
                }
                case TabHelper.DataType.OrderLine: {
                    var orderLine = new OrderLine(tabFile);
                    orderLine.ExportDMTFile(Settings.Default.CSVPath);
                    break;
                }
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}