using CsvHelper.Configuration;

namespace ApiCargar_CVS.Models
{
    public class ClienteMap : ClassMap<Cliente>
    {
        public ClienteMap()
        {
            // Mapeo de las columnas del CSV a las propiedades de la clase Cliente
            Map(m => m.Nombre).Name("nombre"); 
            Map(m => m.Edad).Name("edad");      
            Map(m => m.Email).Name("email");    
        }
    }
}
