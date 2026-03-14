# Especificación del Proyecto: Gym Progress Tracker (MVP)

## 1. Visión General
Aplicación web para el seguimiento de entrenamientos de fuerza, permitiendo registrar ejercicios y las series (peso/repeticiones) realizadas en cada uno.

## 2. Stack Tecnológico
* **Framework:** ASP.NET Core Razor Pages (.NET 10)
* **ORM:** Entity Framework Core (Code First)
* **Base de Datos:** SQLite
* **Entorno:** Linux (Arch)

## 3. Modelo de Datos (MVP)

### Entity: Exercise
* `Id` (int, PK)
* `Name` (string, Required) - Ej: "Sentadilla Libre"
* `MuscleGroup` (string) - Ej: "Pierna"
* `Instructions` (string, Optional)

### Entity: Set
* `Id` (int, PK)
* `ExerciseId` (int, FK) - Relación 1:N con Exercise
* `Weight` (double) - Peso en kg
* `Reps` (int) - Número de repeticiones
* `CreatedAt` (DateTime) - Marca de tiempo del registro

## 4. Alcance del MVP (Funcionalidades)
1. **Gestión de Ejercicios:** - Vista de listado de todos los ejercicios.
   - Formulario de creación de nuevos ejercicios.
2. **Registro de Series:** - Vista de detalle por ejercicio.
   - Formulario para añadir una nueva serie (Peso y Repeticiones) a un ejercicio específico.
3. **Persistencia:** - Configuración de `DbContext` y cadena de conexión a SQLite.
   - Generación y aplicación de Migraciones vía `dotnet ef`.

## 5. Roadmap (Post-MVP)
* **Rutinas:** Agrupar ejercicios en sesiones (ej: "Día A", "Push Day").
* **Gráficos:** Visualización de progresión de carga por ejercicio.
* **Localización:** Al poder ir a distintos gimnasios las máquinas son distintas por lo que se podrá elegir en que gimnasio estás y que se guarden los pesos y se muestren los pesos solo de ese gimnasio.
* **Autenticación:** Sistema de usuarios para perfiles privados.

## 6. Estándares de Código
* Seguir las directivas del archivo `dotnet-framework.instructions.md`.
* Implementar validaciones en el modelo (`DataAnnotations`).
* Uso de Handlers nativos de Razor Pages (`OnGetAsync`, `OnPostAsync`).