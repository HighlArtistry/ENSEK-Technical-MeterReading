using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MeterReadings1.Models
{
    public class MeterReading
    {
        // Unique Identifier
        [Key]
        public int AccountId { get; set; } // Needs to match an existing Accounts AccountId
        public DateTime MeterReadingDateTime { get; set; }
        public int MeterReadValue { get; set; } // Format NNNNN

        public static MeterReading FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');
            MeterReading meterReading = new MeterReading();
            meterReading.AccountId = Convert.ToInt32(values[0]);
            meterReading.MeterReadingDateTime = DateTime.Parse(values[1]);//, "dd-MM-yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
            try
            {
                meterReading.MeterReadValue = Convert.ToInt32(values[2]);
            }
            catch (Exception Ex)
            {
                var catchException = "";
            }
            
            return meterReading;
        }
    }

    
}
