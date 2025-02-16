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
        public IActionResult GetClientes(int page = 1, int size = 10, string? search = null)
        {
            var query = _context.Clientes.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Nombre.Contains(search) || c.Email.Contains(search));
            }

            var totalItems = query.Count();
            var clientes = query.Skip((page - 1) * size).Take(size).ToList();

            return Ok(new { clientes, totalItems });
        }

        // POST api/<ClientesController>
        [HttpPost("upload")]
        [RequestSizeLimit(100_000_000)] // Ajustar si se requiere
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

                using var reader = new StreamReader(file.File.OpenReadStream());
                using var csv = new CsvReader(reader, config);

                csv.Context.RegisterClassMap<ClienteMap>();  // Aplicar el mapeo

                var buffer = new List<Cliente>();
                int totalRegistros = 0;
                const int chunkSize = 1000; // Procesar en bloques de 1000 (por ejemplo)

                while (csv.Read())
                {
                    try
                    {
                        // Lee registro a registro
                        var record = csv.GetRecord<Cliente>();
                        buffer.Add(record);
                        totalRegistros++;

                        // Cuando lleguemos al chunkSize, guardamos en DB
                        if (buffer.Count >= chunkSize)
                        {
                            await _context.Clientes.AddRangeAsync(buffer);
                            await _context.SaveChangesAsync();
                            buffer.Clear();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Capturar y mostrar errores específicos de la fila y columna
                        var errorMessage = $"Error en la fila {csv.Context.Parser?.Row - 1 ?? 0} y columna {csv.Context.Parser?.RawRow - 1 ?? 0}";
                        return BadRequest(new { Message = errorMessage });
                    }
                }

                // Guardar los registros que queden en el buffer
                if (buffer.Count > 0)
                {
                    await _context.Clientes.AddRangeAsync(buffer);
                    await _context.SaveChangesAsync();
                }

                return Ok(new
                {
                    Message = "Archivo procesado y datos guardados correctamente",
                    TotalRegistros = totalRegistros
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error procesando el archivo: {ex.Message}");
            }
        }

    }
}
