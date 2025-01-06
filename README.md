# MonitoreoHortalizasApp
> Aplicación de escritorio para el monitoreo de hortalizas.

## Tabla de contenidos
- [Información general](#información-general)
- [Tecnologías](#tecnologías)
- [Pre-requisitos](#pre-requisitos)
- [Instalación](#instalación)
- [Configuración](#configuración)

## Información general
Esta aplicación de escritorio fue desarrollado durante la residencia profesional en la empresa 
[Instituto Tecnológico Superior de Valladolid](https://www.itsva.edu.mx/), con el objetivo de monitorear el crecimiento 
de hortalizas en camas de cultivo.

## Tecnologías
- C#
- .NET Core 8.0
- Blazor WPF Hybrid
- Blazor Bootstrap 5
- Bootstrap 5
- Dapper
- MySQL 8

## Pre-requisitos
- Visual Studio 2019 o superior
- .NET Core 8.0
- MySQL 8

## Instalación
1. Clonar el repositorio
```bash 
git clone https://github.com/LimbersMay/MonitoreoHortalizas.git
```

2. Abrir el proyecto en Visual Studio 2019 o superior
3. Instalar los paquetes NuGet del proyecto
4. Ejecutar el script de la base de datos `hortalizas.sql` en su servidor MySQL para crear la base de datos y las tablas.

## Configuración
El archivo de configuración se encuentra en el archivo `appsettings.json` en la raíz del proyecto.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=hortalizas;Uid=root;Pwd=123456;"
  }
}
```
Reemplazar los valores de `Server`, `Port`, `Database`, `Uid` y `Pwd` por los valores de su servidor MySQL.