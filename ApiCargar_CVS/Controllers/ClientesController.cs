using System.Globalization;
using ApiCargar_CVS.Data;
using ApiCargar_CVS.Models;
using ApiCargar_CVS.Services;
using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiCargar_CVS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly CsvService _csvService;

        public ClientesController(AppDbContext context, CsvService csvService)
        {
            _context = context;
            _csvService = csvService;
        }

        // GET: api/<ClientesController>
        [HttpGet]
        public async Task<IActionResult> GetClientes([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var clientes = await _context.Clientes
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(clientes);
        }

        // GET api/<ClientesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            return Ok(cliente);
        }

        // POST api/<ClientesController>
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadCsv([FromForm] UploadCsvDto file)
        {
            if (file == null || file.File.Length == 0)
            {
                return BadRequest("Debe subir un archivo CSV válido.");
            }

            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,  // Ignorar validación de encabezados
                    MissingFieldFound = null // Ignorar campos faltantes
                };

                using (var reader = new StreamReader(file.File.OpenReadStream()))
                using (var csv = new CsvReader(reader, config))
                {
                    csv.Context.RegisterClassMap<ClienteMap>();  // Aplicar el mapeo
                    var clientes = csv.GetRecords<Cliente>().ToList();

                    // Guardar en la base de datos
                    await _context.Clientes.AddRangeAsync(clientes);
                    await _context.SaveChangesAsync();

                    return Ok(new { Message = "Archivo procesado y datos guardados correctamente", TotalRegistros = clientes.Count });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error procesando el archivo: {ex.Message}");
            }
        
        }

        // PUT api/<ClientesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ClientesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
