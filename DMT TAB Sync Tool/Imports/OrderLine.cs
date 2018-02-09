using System;
using System.Linq;

namespace TABSync.Imports {
    internal class OrderLine : Import {
        public OrderLine(string path) : base(path) {
            SetHeaders();
            ConvertLines();
        }

        private void SetHeaders() {
            foreach (var column in Enum.GetValues(typeof(Column)).Cast<Column>())
                Headers.Add(column.ToString());
        }

        private void ConvertLines() {
            foreach (var rawLine in rawLines)
                Lines.Add(convertLine(rawLine));
        }

        private string convertLine(string[] rawLine) {
            // Get a list of the column values in the correct order
            var csvLine = Enum.GetValues(typeof(Column)).Cast<Column>().Select(c => getColumnValue(c, rawLine));

            // Escape the fields and join em up
            // "Field", "Field2", "Field3"...
            return string.Join(",", csvLine.Select(s => s.EscapeCsvField('"')));
        }

        /// <summary>
        ///     Retrieves a column from a tab file line
        /// </summary>
        /// <param name="column">Column to retrieve</param>
        /// <param name="rawLine">Line of a TAB file, split on /t. i.e., an array of fields.</param>
        /// <returns>Column value as string</returns>
        private string getColumnValue(Column column, string[] rawLine) {
            try {
                switch (column) {
                    case Column.Company:         return "18867";
                    case Column.OrderNum:        break;
                    case Column.OrderLine:       return rawLine[5];
                    case Column.PartNum:         return rawLine[17];
                    case Column.LineDesc:        return rawLine[19];
                    case Column.RevisionNum:     return string.Empty;
                    case Column.POLine:          return "";
                    case Column.DocUnitPrice:    return rawLine[2];
                    case Column.SellingQuantity: return rawLine[1];
                    case Column.DocDiscount:     return "0";
                    case Column.RequestDate:     return string.Empty;
                    case Column.OrderComment:    return rawLine[22];
                    case Column.NeedByDate:      return string.Empty;
                    default:                     throw new ArgumentOutOfRangeException(nameof(column), column, null);
                }
            } catch (Exception ex) {
                logger.Error($"Failed to locate field '{column}'. Column will be blank. Error: {ex.Message}");
            }
            return string.Empty;
        }

        private enum Column {
            Company,
            OrderNum,
            OrderLine,
            PartNum,
            LineDesc,
            RevisionNum,
            POLine,
            DocUnitPrice,
            SellingQuantity,
            DocDiscount,
            RequestDate,
            OrderComment,
            NeedByDate
        }
    }
}