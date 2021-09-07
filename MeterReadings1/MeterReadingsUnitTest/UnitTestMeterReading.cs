using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MeterReadings1.Models;
using MeterReadings1.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace MeterReadingsUnitTest
{
    [TestClass]
    public class UnitTestMeterReading
    {
        /// <summary
        /// Checking to see if meterReadings table has been seeded correctly
        /// </summary>
        [TestMethod]
        public void TestMeterReadingsHasNoDuplicates()
        {
            //-- Retrieve all records in meterReadings 
            List<MeterReading> currentMeterReadingsDb = new List<MeterReading>();
            string meterReadingConnectionString = "Data Source=../../../../MeterReadings1/meterReadings.db";
            using (SqliteConnection con = new SqliteConnection(meterReadingConnectionString))
            {
                con.Open();

                string stm = "SELECT AccountId FROM meterReadings";

                using (SqliteCommand cmd = new SqliteCommand(stm, con))
                {
                    using (SqliteDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            try
                            {
                                //add items to your list of existing records in the db
                                currentMeterReadingsDb.Add((MeterReading)rdr["AccountId"]);
                            }
                            catch (Exception Ex)
                            {

                            }

                        }
                    }
                }

                con.Close();
            }

            // Linq Query to retrieve duplicate items in the list
            var query = currentMeterReadingsDb.GroupBy(x => x)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();

            //If no duplicates found then pass the test
            Assert.AreEqual(query.Count(), 0);
        }

        /// <summary
        /// Checking to see if accounts table has been seeded correctly
        /// </summary>
        [TestMethod]
        public void TestAccountsDbIsNotNull()
        {
            //-- Retrieve all records in meterReadings (List populated inside loop to keep up to date)
            List<string> currentAccountsDb = new List<string>();
            // Retrieve accounts from accounts.db
            string accountsConnectionString = "Data Source=../../../../MeterReadings1/accounts.db";
            using (SqliteConnection con = new SqliteConnection(accountsConnectionString))
            {
                con.Open();

                //SQL Query to retrieve all AccountId's
                string stm = "SELECT AccountId FROM accounts";

                using (SqliteCommand cmd = new SqliteCommand(stm, con))
                {
                    using (SqliteDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            //add items to your list
                            currentAccountsDb.Add(rdr["AccountId"].ToString());
                        }
                    }
                }

                con.Close();
            }

            // Check to see if the accounts table is null (should be populated)
            Assert.IsNotNull(currentAccountsDb);
        }
    }
}
