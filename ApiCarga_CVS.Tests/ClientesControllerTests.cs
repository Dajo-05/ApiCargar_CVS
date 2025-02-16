using ApiCargar_CVS.Controllers;
using ApiCargar_CVS.Data;
using ApiCargar_CVS.Models;
using ApiCargar_CVS.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ApiCargar_CVS.Tests
{
    public class ClientesControllerTests
    {
        private readonly AppDbContext _context;
        private readonly Mock<CsvService> _mockCsvService;
        private readonly ClientesController _controller;

        public ClientesControllerTests()
        {
            // 1. Configurar el DbContext para usar InMemory
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;

            _context = new AppDbContext(options);

            // 2. Crear un Mock de CsvService, pasando el contexto real
            _mockCsvService = new Mock<CsvService>(_context);
            
            // 3. Instanciar el controlador con el contexto y el servicio mockeado
            _controller = new ClientesController(_context, _mockCsvService.Object);
        }

        [Fact]
        public async Task GetClientes_ReturnsOkResult_WithListOfClientes()
        {
            // Arrange
            // Sembramos datos en la base de datos InMemory
            _context.Clientes.AddRange(new List<Cliente>
            {
                new Cliente { Id = 1, Nombre = "Cliente1", Email = "cliente1@example.com" },
                new Cliente { Id = 2, Nombre = "Cliente2", Email = "cliente2@example.com" }
            });
            _context.SaveChanges();

            // Act
            var result = _controller.GetClientes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = okResult.Value;

            // Verificar que returnValue no es nulo
            Assert.NotNull(returnValue);

        }

        [Fact]
        public async Task UploadCsv_ReturnsBadRequest_WhenFileIsNull()
        {
            // Act
            var result = await _controller.UploadCsv(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Debe subir un archivo CSV válido.", badRequestResult.Value);
        }

        [Fact]
        public async Task UploadCsv_ReturnsBadRequest_WhenFileHasErrors()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var content = "Id,Nombre,Email\n1,Cliente1,cliente1@example.com\n2,Cliente2,cliente2@example.com\n3,Cliente3,invalid-email";
            var fileName = "test.csv";

            // Escribimos 'content' en un MemoryStream para simular el archivo
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            mockFile.Setup(_ => _.OpenReadStream()).Returns(ms);
            mockFile.Setup(_ => _.FileName).Returns(fileName);
            mockFile.Setup(_ => _.Length).Returns(ms.Length);

            var uploadCsvDto = new UploadCsvDto { File = mockFile.Object };

            // Act
            var result = await _controller.UploadCsv(uploadCsvDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Error en la fila", badRequestResult.Value.ToString());
        }
    }
}
