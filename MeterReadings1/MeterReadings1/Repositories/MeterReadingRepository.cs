using MeterReadings1.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeterReadings1.Repositories
{
    public class MeterReadingRepository : IMeterReadingRepository
    {
        //This repository will query the db using the MeterReadingContext
        private readonly MeterReadingContext _context;

        public MeterReadingRepository(MeterReadingContext context)
        {
            _context = context;
        }

        public async Task<MeterReading> Create(MeterReading meterReading)
        {
            //Add new instance of the MeterReading class
            _context.MeterReadings.Add(meterReading);
            // Insert data into the db
            await _context.SaveChangesAsync();

            return meterReading;
        }

        public async Task Delete(int id)
        {
            // Retrieve record based on provided Id
            var meterReadingToDelete = await _context.MeterReadings.FindAsync(id);
            _context.MeterReadings.Remove(meterReadingToDelete);
            // Delete record from db
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MeterReading>> Get()
        {
            // Fetch all MeterReading's from db
            return await _context.MeterReadings.ToListAsync();
        }

        public async Task<MeterReading> Get(int id)
        {
            // Fetch MeterReading from db based on provided id
            return await _context.MeterReadings.FindAsync(id);
        }

        public async Task Update(MeterReading meterReading)
        {
            //Update MeterReading record
            _context.Entry(meterReading).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
