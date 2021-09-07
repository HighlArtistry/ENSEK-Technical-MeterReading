using MeterReadings1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeterReadings1.Repositories
{
    public interface IMeterReadingRepository
    {
        Task<IEnumerable<MeterReading>> Get();
        Task<MeterReading> Get(int id);
        Task<MeterReading> Create(MeterReading meterReading);
        Task Update(MeterReading meterReading);
        Task Delete(int id);
    }
}
