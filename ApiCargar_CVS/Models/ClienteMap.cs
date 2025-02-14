using CsvHelper.Configuration;

namespace ApiCargar_CVS.Models
{
    public class ClienteMap : ClassMap<Cliente>
    {
        public ClienteMap()
        {
            Map(m => m.Nombre).Name("nombre");  // Mapea "nombre" del CSV a la propiedad Nombre
            Map(m => m.Edad).Name("edad");      // Mapea "edad" del CSV a la propiedad Edad
            Map(m => m.Email).Name("email");    // Mapea "email" del CSV a la propiedad Email
        }
    }
}
