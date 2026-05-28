# Plan: Gym Tracker MVP — Razor Pages + EF Core + SQLite

El objetivo es construir el MVP completo desde el proyecto vacío: capa de datos con EF Core Code First, páginas para gestionar ejercicios y registrar series, y navegación funcional.

---

## Phase 1 — Packages (tú los ejecutas)

1. Ejecutar en terminal antes de que el agente empiece:
   ```
   dotnet add package Microsoft.EntityFrameworkCore.Sqlite
   dotnet add package Microsoft.EntityFrameworkCore.Design
   ```

---

## Phase 2 — Capa de Datos *(pasos independientes 2–4, paralelos)*

2. **Crear `Data/Entities/Exercise.cs`** — Entidad con `Id`, `Name` ([Required, MaxLength(100)]), `MuscleGroup` (string?, MaxLength(50)), `Instructions` (string?). XML docs. Namespace `GymTracker.Data.Entities`.

3. **Crear `Data/Entities/Set.cs`** — Entidad con `Id`, `ExerciseId` (FK int), `Weight` (double, [Range(0,2000)]), `Reps` (int, [Range(1,999)]), `CreatedAt` (**DateTimeOffset** — per instructions). Navigation property `Exercise?`. XML docs.

4. **Crear `Data/AppDbContext.cs`** — Hereda `DbContext`. Primary constructor. `DbSet<Exercise>`, `DbSet<Set>`. `OnModelCreating`: cascade delete Exercise→Sets.

5. **Modificar `appsettings.json`** — Añadir `"ConnectionStrings": { "DefaultConnection": "Data Source=gymtracker.db" }`.

6. **Modificar `Program.cs`** — Registrar `AddDbContext<AppDbContext>` con `UseSqlite`. Tras `app.Build()`: scope que llama `db.Database.MigrateAsync()` para auto-migrar al arranque.

---

## Phase 3 — Migración (tú la ejecutas)

7. Ejecutar tras los pasos 2–6:
   ```
   dotnet ef migrations add InitialCreate
   ```
   Genera `Migrations/` con el snapshot del modelo.

---

## Phase 4 — Feature: Ejercicios *(pasos 8–11, paralelos)*

8. **Crear `Pages/Exercises/Index.cshtml.cs`** — Inyección de `AppDbContext` via primary constructor. `OnGetAsync` carga `IList<Exercise>` con `ToListAsync()`.

9. **Crear `Pages/Exercises/Index.cshtml`** — Ruta `/Exercises`. Tabla: Nombre, Grupo Muscular, link "Ver" a Detail. Botón "Nuevo Ejercicio" → `/Exercises/Create`.

10. **Crear `Pages/Exercises/Create.cshtml.cs`** — `[BindProperty] Exercise Input`. `OnPostAsync`: valida `ModelState`, guarda, redirige a `Detail/{id}`. Post-Redirect-Get.

11. **Crear `Pages/Exercises/Create.cshtml`** — Ruta `/Exercises/Create`. Formulario: Name (required), MuscleGroup, Instructions (textarea). Tag helpers de validación.

---

## Phase 5 — Feature: Detalle + Series *(depende de Phase 2 y 3)*

12. **Crear `Pages/Exercises/Detail.cshtml.cs`** — Ruta `/Exercises/Detail/{id:int}`. `OnGetAsync`: carga `Exercise` + `IList<Set>` del ejercicio ordenadas por `CreatedAt` desc. `[BindProperty] NewSet` (Weight + Reps). `OnPostAsync`: crea `Set { ExerciseId=id, CreatedAt=DateTimeOffset.UtcNow }`, guarda, redirige a la misma página (PRG).

13. **Crear `Pages/Exercises/Detail.cshtml`** — Sección superior: nombre, grupo, instrucciones. Tabla de series: Fecha, Peso (kg), Reps. Formulario al pie para añadir serie.

---

## Phase 6 — Navegación y Layout *(paralelo)*

14. **Modificar `Pages/Shared/_Layout.cshtml`** — Añadir link de nav "Ejercicios" → `/Exercises`.

15. **Modificar `Pages/Index.cshtml`** — Reemplazar placeholder defecto con welcome card y botón/link a `/Exercises`.

---

## Archivos relevantes

- `GymTracker.csproj` — SDK-style .NET 10, Nullable/ImplicitUsings ya habilitados
- `Program.cs` — punto de registro de servicios y middleware
- `Pages/Shared/_Layout.cshtml` — navbar de Bootstrap ya estructurado
- `appsettings.json` — donde va la connection string

---

## Verificación

1. `dotnet build` — sin errores tras todos los cambios de código
2. `dotnet ef migrations add InitialCreate` — genera `Migrations/` correctamente
3. `dotnet run` — app arranca, `gymtracker.db` creado automáticamente
4. Navegar `/Exercises` → tabla vacía sin error 500
5. Crear ejercicio → aparece en lista, redirige a detalle
6. En detalle: formulario de serie visible; registrar → aparece en tabla, recarga misma página

---

## Decisiones fijadas

- C# 13 / .NET 10 moderno (se ignoran restricciones C# 7.3 del instructions, propias de .NET Framework legacy)
- Se aplican las partes agnósticas: `DateTimeOffset` para timestamps, `async/await`, logging estructurado, XML docs en entidades
- La página de detalle hace de hub para ver y registrar series (todo en uno)
