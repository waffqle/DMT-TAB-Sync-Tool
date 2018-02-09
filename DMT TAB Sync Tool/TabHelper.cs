using System;
using System.IO;
using TABSync.Properties;

namespace TABSync {
    internal static class TabHelper {
        public static DataType GetFileType(string tabFile) {
            var file = Path.GetFileName(tabFile) ?? string.Empty;
            
            if (file.StartsWith(Settings.Default.OrderHeadPrefix))
                return DataType.OrderHead;
            if (file.StartsWith(Settings.Default.OrderLinePrefix))
                return DataType.OrderLine;

            throw new ArgumentException("Unrecognized file type.");
        }

        public enum DataType
        {
            OrderHead,
            OrderLine
        }
    }
}