# GymTracker

GymTracker es una aplicación web para registrar y gestionar tus rutinas de entrenamiento en el gimnasio. Permite llevar un seguimiento detallado de los ejercicios realizados, las series, repeticiones y pesos utilizados, facilitando el progreso y la organización de tus entrenamientos.

## Características principales
- Registro de ejercicios y series con peso y repeticiones
- Visualización de historial de entrenamientos
- Gestión de grupos musculares y descripciones de ejercicios
- Interfaz intuitiva y fácil de usar

## Tecnologías utilizadas
- ASP.NET Core (.NET 10)
- Razor Pages
- Entity Framework Core
- Bootstrap 5
- Docker (opcional)

## Instalación y ejecución

### Requisitos previos
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (opcional, para despliegue en contenedores)

### Ejecución local
1. Clona el repositorio:
   ```bash
   git clone https://github.com/tu-usuario/GymTracker.git
   cd GymTracker
   ```
2. Restaura las dependencias:
   ```bash
   dotnet restore
   ```
3. Aplica las migraciones y ejecuta la aplicación:
   ```bash
   dotnet ef database update
   dotnet run
   ```
4. Accede a la aplicación en [http://localhost:5000](http://localhost:5000) o el puerto configurado.

### Uso con Docker
1. Construye la imagen:
   ```bash
   docker build -t gymtracker .
   ```
2. Ejecuta el contenedor:
   ```bash
   docker-compose up
   ```

## Estructura del proyecto
- `Pages/` - Vistas Razor y lógica de páginas
- `Data/` - Modelos de datos y contexto de base de datos
- `DTO/` - Objetos de transferencia de datos
- `Migrations/` - Migraciones de base de datos
- `wwwroot/` - Archivos estáticos (CSS, JS, librerías)

## Contribuciones
¡Las contribuciones son bienvenidas! Por favor, abre un issue o pull request para sugerencias o mejoras.

## Licencia
Este proyecto está bajo la licencia MIT.
