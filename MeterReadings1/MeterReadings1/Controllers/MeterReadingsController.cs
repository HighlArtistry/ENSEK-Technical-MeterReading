using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using MeterReadings1.Models;
using MeterReadings1.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace MeterReadings1.Controllers
{
    [Route("api/meter-reading-uploads")] //Defines path that controller will point to
    public class MeterReadingsController : Controller//Base
    {
        // Needs instyance of MeterReadingRepository to interact with the db
        private readonly IMeterReadingRepository _meterReadingRepository;

        public MeterReadingsController(IMeterReadingRepository meterReadingRepository)
        {
            _meterReadingRepository = meterReadingRepository;
        }

        #region meterReading CRUD

        //[HttpGet]
        //public async Task<IEnumerable<MeterReading>> GetMeterReadings()
        //{
        //    return await _meterReadingRepository.Get();
        //}

        [HttpGet("{id}")]
        public async Task<ActionResult<MeterReading>> GetMeterReadings(int id) // ActionResult provides ability to return multiple types like NootFound or BadRequest
        {
            return await _meterReadingRepository.Get(id);
        }

        //[HttpPost]
        //public async Task<ActionResult<MeterReading>> PostMeterReadings([FromBody] MeterReading meterReading) //Asp.NET ModelBinding will convert Json Payload in body into an Account object
        //{
        //    var newMeterReading = await _meterReadingRepository.Create(meterReading);

        //    return CreatedAtAction(nameof(GetMeterReadings), new { id = newMeterReading.AccountId }, newMeterReading);
        //}

        [HttpPut]
        public async Task<ActionResult<MeterReading>> PutMeterReadings(int id, [FromBody] MeterReading meterReading)
        {
            if (id != meterReading.AccountId)
            {
                return BadRequest();
            }
            else
            {
                await _meterReadingRepository.Update(meterReading);

                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<MeterReading>> DeleteMeterReadings(int id)
        {
            var meterReadingToDelete = await _meterReadingRepository.Get(id);
            if (meterReadingToDelete == null)
            {
                return NotFound();
            }
            else
            {
                await _meterReadingRepository.Delete(id);

                return NoContent();
            }
        }
        #endregion


        #region CSV Actions
        [HttpGet]
        public IActionResult Index(List<MeterReading> meterReadings = null)
        {
            meterReadings = meterReadings == null ? new List<MeterReading>() : meterReadings;
            return View(meterReadings);
        }

        [HttpPost]
        public IActionResult Index(IFormFile file, [FromServices] IWebHostEnvironment hostingEnvironment)
        {

            #region Saving file on System!

            var path = $"{Directory.GetCurrentDirectory()}{@"\files"}";
            var webRootpath = $"{hostingEnvironment.WebRootPath}";
            string uploadsFolderPath = Path.Combine(hostingEnvironment.WebRootPath, "uploads");
            string filePath = Path.Combine(uploadsFolderPath, file.FileName);
            using (Stream fileStream = System.IO.File.Create(filePath))//new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }

            #endregion

            //string fileName = $"{hostingEnvironment.WebRootPath}\\files\\{file.FileName}";

            var meterReadings = this.GetMeterReadingsList(filePath);
            return Index(meterReadings);
        }

        /// <summary
        /// This method is used to return a List of successful meter readings that have been added to the database
        /// </summary>
        private List<MeterReading> GetMeterReadingsList(string path)
        {
            int result = 0;
            List<MeterReading> meterReadings = new List<MeterReading>();
            List<MeterReading> meterReadings2 = new List<MeterReading>();
            List<MeterReading> currentMeterReadingsDb = new List<MeterReading>();

            List<string> accountsList = new List<string>();

            #region Bring AccountsSQLite into a list - fetch seeded records


            // Retrieve accounts from accounts.db
            using (SqliteConnection con = new SqliteConnection("Data Source=accounts.db"))
            {
                con.Open();

                string stm = "SELECT AccountId FROM accounts";

                using (SqliteCommand cmd = new SqliteCommand(stm, con))
                {
                    using (SqliteDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            //add items to your list
                            accountsList.Add(rdr["AccountId"].ToString());
                        }
                    }
                }

                con.Close();
            }

            #endregion

            #region Read CSV

            /// Parse all records from the uploaded csv into a MeterReading list (using LINQ)
            meterReadings = System.IO.File.ReadAllLines(path)
                                       .Skip(1) // Skip the column headers row in the csv
                                       .Select(v => MeterReading.FromCsv(v)) // Use the MeterReading object to arrange the fields from the csv
                                       .ToList();

            foreach (var record in meterReadings)
            {

                foreach (var accountAccIdValue in accountsList)
                {
                    // Validation of uploaded csv:
                    if (record.AccountId == Convert.ToInt32(accountAccIdValue))
                    {
                        int positiveMeterReadValue = Math.Abs(record.MeterReadValue);
                        if ((positiveMeterReadValue.ToString().Length) == 5)
                        {


                            //-- Retrieve all records in meterReadings (List populated inside loop to keep up to date)
                            using (SqliteConnection con = new SqliteConnection("Data Source=meterReadings.db"))
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

                            // Final Validations:
                            if (currentMeterReadingsDb.Count() > 0)
                            {
                                // If the meterReadings database is currently not empty, check to see if record already exists by AccountId
                                foreach (var existingMeterReadin in currentMeterReadingsDb)
                                {
                                    if (record.AccountId != existingMeterReadin.AccountId)
                                    {
                                        // The validation-passing record does not already exist in db
                                        // Add Validated results to meterReadings.db (Add as a new entry in MeterReadings.db)
                                        // Increment Counter for successful uploads

                                        result++;
                                        meterReadings2.Add(record);
                                        var newMeterReading = _meterReadingRepository.Create(record);

                                    }
                                }
                            }
                            else
                            {
                                // Else If the meterReadings database is currently empty
                                result++;
                                meterReadings2.Add(record);
                                var newMeterReading = _meterReadingRepository.Create(record);
                            }

                        }
                    }
                }

            }

            #endregion

            // Make counter accessible from index.cshtml for failed records number
            ViewBag.Scrap = meterReadings.Count() - meterReadings2.Count();

            return meterReadings2;
        }
        #endregion
    }
}
