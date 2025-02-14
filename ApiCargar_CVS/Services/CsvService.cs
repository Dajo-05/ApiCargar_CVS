using ApiCargar_CVS.Data;
using ApiCargar_CVS.Models;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;

namespace ApiCargar_CVS.Services
{
    public class CsvService
    {
        private readonly AppDbContext _context;

        public CsvService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ProcesarCsvAsync(Stream fileStream)
        {
            using var reader = new StreamReader(fileStream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

            var records = csv.GetRecords<Cliente>().ToList();

            await _context.Clientes.AddRangeAsync(records);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
