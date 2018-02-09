using System;
using System.Linq;

namespace TABSync.Imports {
    internal class OrderHead : Import {
        public OrderHead(string path) : base(path) {
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
                var newCustomer = rawLine[7]=="1";
                var useOTS      = newCustomer;
                switch (column) {
                    case Column.Company:         return "18867";
                    case Column.OrderNum:        return "0";
                    case Column.CustomerCustID:  return rawLine[13];
                    case Column.BTCustNumCustID: return rawLine[13];
                    case Column.PONum:           return rawLine[27];
                    case Column.EntryPerson:     return rawLine[31];
                    case Column.ShipToNum:       return rawLine[36];
                    case Column.RequestDate:     return rawLine[2];
                    case Column.OrderDate:       return rawLine[1];
                    case Column.FOB:             return string.Empty;
                    case Column.ShipViaCode:     return string.Empty;
                    case Column.TermsCode:       return rawLine[28];
                    case Column.OrderComment:    return rawLine[53];
                    case Column.NeedByDate:      return rawLine[2];
                    case Column.CurrencyCode:    return GetCurrencyCode(CountryOfOrigin);
                    case Column.UseOTS:          return useOTS.ToString();
                    case Column.OTSName:         return useOTS ? rawLine[37] : string.Empty;
                    case Column.OTSAddress1:     return useOTS ? rawLine[38] : string.Empty;
                    case Column.OTSAddress2:     return useOTS ? rawLine[39] : string.Empty;
                    case Column.OTSAddress3:     return string.Empty;
                    case Column.OTSCity:         return useOTS ? rawLine[40] : string.Empty;
                    case Column.OTSState:        return useOTS ? rawLine[41] : string.Empty;
                    case Column.OTSZIP:          return useOTS ? rawLine[43] : string.Empty;
                    case Column.OTSContact:      return string.Empty;
                    case Column.SalesRepCode1:   return string.Empty;
                    case Column.ShipToCustId:    return string.Empty;
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
            CustomerCustID,
            BTCustNumCustID,
            PONum,
            EntryPerson,
            ShipToNum,
            RequestDate,
            OrderDate,
            FOB,
            ShipViaCode,
            TermsCode,
            OrderComment,
            NeedByDate,
            CurrencyCode,
            UseOTS,
            OTSName,
            OTSAddress1,
            OTSAddress2,
            OTSAddress3,
            OTSCity,
            OTSState,
            OTSZIP,
            OTSContact,
            SalesRepCode1,
            ShipToCustId
        }
    }
}