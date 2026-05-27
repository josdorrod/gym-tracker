## Plan: Implementar Planes de Entrenamiento, Ubicaciones y Sesiones

Este plan introduce la capacidad de crear planes de entrenamiento, registrar ubicaciones (gimnasios) y agrupar los sets de ejercicios en sesiones de entrenamiento, mejorando la estructura y el seguimiento de la actividad física.

### Fase 1: Extender el Modelo de Datos

Crearemos las nuevas entidades y actualizaremos las existentes para soportar las nuevas funcionalidades.

**Pasos**
1.  **Crear la entidad `Location`**:
    *   Crea el archivo `Data/Entities/Location.cs`.
    *   Añade propiedades: `Id` (int), `Name` (string, requerido), `Address` (string, opcional).

2.  **Crear la entidad `Plan`**:
    *   Crea el archivo `Data/Entities/Plan.cs`.
    *   Añade propiedades: `Id` (int), `Name` (string, requerido), `Description` (string, opcional).
    *   Añade una colección de `Exercise` para una relación muchos a muchos: `public ICollection<Exercise> Exercises { get; set; }`.

3.  **Crear la entidad `WorkoutSession`**:
    *   Crea el archivo `Data/Entities/WorkoutSession.cs`.
    *   Añade propiedades: `Id` (int), `Date` (DateTime), `LocationId` (int, FK a `Location`), `PlanId` (int?, FK a `Plan`, opcional).
    *   Añade propiedades de navegación: `public Location Location { get; set; }`, `public Plan Plan { get; set; }`.
    *   Añade una colección de `Set`: `public ICollection<Set> Sets { get; set; }`.

4.  **Actualizar la entidad `Set`**:
    *   En `Data/Entities/Set.cs`, añade la clave foránea y la propiedad de navegación para `WorkoutSession`:
        *   `public int? WorkoutSessionId { get; set; }` (nullable para no romper los sets existentes).
        *   `public WorkoutSession? WorkoutSession { get; set; }`.

5.  **Actualizar `GymTrackerDbContext`**:
    *   En `Data/GymTrackerDbContext.cs`, añade los nuevos `DbSet`:
        *   `public DbSet<Location> Locations { get; set; }`
        *   `public DbSet<Plan> Plans { get; set; }`
        *   `public DbSet<WorkoutSession> WorkoutSessions { get; set; }`
    *   Configura la relación muchos a muchos entre `Plan` y `Exercise`.

6.  **Crear y aplicar la migración de la base de datos**:
    *   Ejecuta el comando para añadir una nueva migración (ej. `dotnet ef migrations add AddPlansAndSessions`).
    *   Aplica la migración a la base de datos (ej. `dotnet ef database update`).

### Fase 2: Crear Servicios y Lógica de Negocio

Implementaremos los servicios para gestionar las nuevas entidades.

**Pasos**
1.  **Crear servicios para `Location` y `Plan`**:
    *   Define las interfaces `ILocationService.cs` y `IPlanService.cs` en `Services/`.
    *   Implementa `LocationService.cs` y `PlanService.cs` con métodos CRUD básicos (Crear, Leer, Actualizar, Borrar).

2.  **Crear servicio para `WorkoutSession`**:
    *   Define la interfaz `IWorkoutSessionService.cs`.
    *   Implementa `WorkoutSessionService.cs`. Este servicio gestionará el inicio y obtención de sesiones de entrenamiento.

3.  **Actualizar `SetService`**:
    *   Modifica `CreateSetAsync` en `Services/SetService.cs` para que acepte un `workoutSessionId` y lo asigne al nuevo `Set`.

4.  **Registrar los servicios**:
    *   En `Program.cs`, registra los nuevos servicios (`ILocationService`, `IPlanService`, `IWorkoutSessionService`) en el contenedor de inyección de dependencias.

### Fase 3: Desarrollar las Páginas Razor

Crearemos las interfaces de usuario para gestionar las nuevas entidades y modificaremos el flujo existente.

**Pasos**
1.  **Crear páginas CRUD para `Location`**:
    *   Crea una nueva carpeta `Pages/Locations/`.
    *   Implementa las páginas `Index.cshtml`, `Create.cshtml`, `Edit.cshtml` y `Delete.cshtml` para gestionar las ubicaciones.

2.  **Crear páginas CRUD para `Plan`**:
    *   Crea una nueva carpeta `Pages/Plans/`.
    *   Implementa las páginas `Index.cshtml`, `Create.cshtml`, `Edit.cshtml` para gestionar los planes. La página de edición permitirá añadir o quitar ejercicios del plan.

3.  **Crear página de `WorkoutSession`**:
    *   Crea una página `Pages/Workout/Start.cshtml` donde el usuario pueda iniciar una nueva sesión de entrenamiento seleccionando una `Location` y opcionalmente un `Plan`.
    *   Al iniciar, se crea un registro en `WorkoutSession` y se redirige al usuario a una página de "sesión activa".

4.  **Modificar el flujo de añadir `Set`**:
    *   La página de detalle del ejercicio (`Pages/Exercises/Detail.cshtml.cs`) deberá comprobar si hay una sesión activa.
    *   Si hay una sesión activa, al crear un `Set`, se asociará a esa sesión.
    *   Si no hay sesión, se podría mantener el comportamiento actual o pedir al usuario que inicie una.

5.  **Añadir navegación en la UI**:
    *   En el archivo de layout `Pages/Shared/_Layout.cshtml`, añade enlaces en la barra de navegación para ir a los índices de `Locations` y `Plans`.

### Verificación
1.  **Verificar CRUD de `Location`**: Navega a la página de ubicaciones y comprueba que puedes crear, ver, editar y eliminar una ubicación.
2.  **Verificar CRUD de `Plan`**: Navega a la página de planes. Comprueba que puedes crear un plan y añadirle ejercicios existentes.
3.  **Verificar inicio de sesión**: Ve a la página para iniciar una sesión, selecciona una ubicación y un plan, e inicia la sesión. Verifica que se crea un registro en la tabla `WorkoutSessions`.
4.  **Verificar asociación de `Set`**: Con una sesión activa, ve a un ejercicio y añade un nuevo `Set`. Comprueba en la base de datos que el nuevo `Set` tiene el `WorkoutSessionId` correcto.
5.  **Verificar navegación**: Asegúrate de que los nuevos enlaces en el menú principal funcionan correctamente.
