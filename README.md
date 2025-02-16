# ApiCargar_CVS

Este proyecto es una API para gestionar clientes y cargar datos desde archivos CSV.

## Requisitos

- .NET 8 SDK.
- SQL Server.

## Instrucciones de Instalación

### Clonar el Repositorio

Primero, clona el repositorio en tu máquina local:
git clone https://github.com/tu-usuario/ApiCargar_CVS.git 

cd ApiCargar_CVS

### Configurar la Base de Datos

Asegúrate de tener una base de datos SQL Server en funcionamiento. Luego, configura la cadena de conexión en el archivo `appsettings.json`:
```
{ "ConnectionStrings": { "ConexionBD": "Server=tu-servidor;Database=tu-base-de-datos;Trusted_Connection=true;TrustServerCertificate=true;" } }
```

## Nota: se sugiere que el valor de Database=tu-base-de-datos; sea por el nombre que se uso para el proyecto que es Database=ClientesBD, el cual ya esta configurado en el proyecto por defecto ya que se fue el nombre de base de datos que se uso para el desarrollo y prueba del mismo, de igual manera se puede usar cualquier otro nombre de base de datos.


### Restaurar Dependencias

Restaura las dependencias del proyecto utilizando el siguiente comando:
```
dotnet restore
```

### Aplicar Migraciones

Aplica las migraciones para crear la base de datos y las tablas necesarias:
```
dotnet ef database update
```

### Configuración de URL para Corss
Se debe editar el archivo Program.cs para no tener bloqueo de Corss desde el backend hacia el fronted, cambiando el valor de TU_URL por la url que genere el proyecto frontend al ejecutarlo.
```
//Configuracion de Corss
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("TU_URL")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```


### Ejecutar el Proyecto

Ejecuta el proyecto utilizando el siguiente comando:
```
dotnet run
```






