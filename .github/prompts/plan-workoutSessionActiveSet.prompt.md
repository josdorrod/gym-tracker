## Plan: Sesion activa antes de añadir sets

Implementar un flujo explícito de inicio de sesión de entrenamiento y hacer que el alta de sets dependa de una sesión activa del día actual. La recomendación es considerar activa cualquier WorkoutSession creada hoy, reutilizar la más reciente del día, y cuando no exista forzar la creación desde una página dedicada donde la localización sea obligatoria y el plan opcional.

**Steps**
1. Definir el contrato de sesión activa en un nuevo servicio `IWorkoutSessionService` y su implementación `WorkoutSessionService`: método para obtener la sesión activa de hoy, método para crear una nueva sesión con localización obligatoria y plan opcional, y DTO/view model mínimo para exponer el estado actual. Esta es la pieza base y bloquea los pasos 2, 3 y 4.
2. Ajustar el modelo de dominio para soportar el concepto de sesión activa diaria sin ambigüedad. Mantener la regla acordada: activa = cualquier `WorkoutSession` creada hoy. Para que el criterio sea consistente y barato de consultar, conviene normalizar el filtro sobre `StartTime.Date` en UTC o, preferiblemente, añadir una columna derivada/criterio homogéneo de fecha si al implementar aparecen problemas de traducción SQL. Este paso depende de 1 solo si se decide introducir helpers o cambios de persistencia; en caso contrario puede hacerse en paralelo con 1.
3. Crear la página Razor de inicio de sesión, por ejemplo `/Pages/WorkoutSessions/Start.cshtml` y su PageModel, con selector obligatorio de localización (`ILocationService.GetAllLocationsAsync`) y selector opcional de plan (`IPlanService.GetAllPlansAsync`). El POST debe validar entrada, crear la sesión y redirigir al flujo de trabajo posterior. Este paso depende de 1 y puede avanzar en paralelo con 4.
4. Integrar el chequeo de sesión activa en el flujo de sets: en `/Pages/Exercises/Detail.cshtml.cs`, consultar la sesión activa de hoy en `OnGetAsync` y `OnPostAsync`; si no existe, impedir el alta del set y redirigir a la nueva página de inicio de sesión con retorno al ejercicio actual. Si existe, `SetService.CreateSetAsync` debe asociar el `WorkoutSessionId` al nuevo `Set`. Este paso depende de 1.
5. Extender `ISetService` y `SetService` para que el alta del set deje de ser ciega respecto a la sesión. La firma debería aceptar el `workoutSessionId` o resolverlo internamente a través de `IWorkoutSessionService`; la opción preferible es resolver la sesión en la capa PageModel y pasar el identificador al servicio de sets para mantener responsabilidades separadas. Este paso depende de 4.
6. Añadir navegación y affordances de UI: enlace visible a iniciar sesión de entrenamiento y estado de “sesión activa” en la pantalla de detalle del ejercicio para que el usuario entienda por qué puede o no registrar una serie. Este paso depende de 3 y 4.
7. Cubrir el cambio con tests focalizados. Añadir tests de servicio para `WorkoutSessionService` (reutiliza sesión de hoy, crea nueva sesión con localización, admite plan nulo) y actualizar tests de `/GymTracker.Tests/Exercises/DetailModelTests.cs` para verificar redirección cuando no hay sesión activa y creación de set cuando sí la hay. Este paso depende de 1, 3, 4 y 5.

**Relevant files**
- `/home/josdorrod/Documentos/Programacion/projects/GymTracker/Pages/Exercises/Detail.cshtml.cs` — punto de entrada actual para añadir sets; aquí debe comprobarse la sesión activa y manejar la redirección.
- `/home/josdorrod/Documentos/Programacion/projects/GymTracker/Pages/Exercises/Detail.cshtml` — formulario actual de alta de sets; debe reflejar el estado de sesión activa y ofrecer enlace para iniciarla.
- `/home/josdorrod/Documentos/Programacion/projects/GymTracker/Services/ISetService.cs` — contrato a ampliar para recibir o coordinar la sesión activa.
- `/home/josdorrod/Documentos/Programacion/projects/GymTracker/Services/SetService.cs` — persistencia del set; aquí se asignará `WorkoutSessionId` al crear la serie.
- `/home/josdorrod/Documentos/Programacion/projects/GymTracker/Data/Entities/Set.cs` — ya contiene `WorkoutSessionId`; sirve como soporte del enlace entre serie y sesión.
- `/home/josdorrod/Documentos/Programacion/projects/GymTracker/Data/Entities/WorkoutSession.cs` — modelo de sesión; aquí se validará si el shape actual basta o si hay que ajustar `EndTime`/timestamps para no confundir “sesión del día” con “sesión cerrada”.
- `/home/josdorrod/Documentos/Programacion/projects/GymTracker/Data/GymTrackerDbContext.cs` — relaciones EF ya existentes entre `WorkoutSession`, `Set`, `Plan` y `Location`; posible punto de ajuste si se añade soporte de consulta diaria más robusta.
- `/home/josdorrod/Documentos/Programacion/projects/GymTracker/Services/ILocationService.cs` — fuente del selector obligatorio de localizaciones en la nueva página.
- `/home/josdorrod/Documentos/Programacion/projects/GymTracker/Services/IPlanService.cs` — fuente del selector opcional de planes activos.
- `/home/josdorrod/Documentos/Programacion/projects/GymTracker/Program.cs` — registro del nuevo `IWorkoutSessionService`.
- `/home/josdorrod/Documentos/Programacion/projects/GymTracker/GymTracker.Tests/Exercises/DetailModelTests.cs` — tests a extender para el comportamiento de redirección y creación condicionada por sesión activa.
- `/home/josdorrod/Documentos/Programacion/projects/GymTracker/GymTracker.Tests/Locations/LocationServiceTests.cs` — referencia del patrón de tests de servicios con EF InMemory.

**Verification**
1. Añadir tests unitarios del nuevo `WorkoutSessionService` para comprobar: reutiliza la sesión más reciente de hoy; no reutiliza sesiones de otro día; crea sesión con `LocationId` obligatorio y `PlanId` opcional.
2. Actualizar tests de `DetailModel` para comprobar: sin sesión activa, `OnPostAsync` redirige a la página de inicio de sesión; con sesión activa, invoca `CreateSetAsync` con el `workoutSessionId` esperado.
3. Ejecutar los tests de la solución o, como mínimo, los de `GymTracker.Tests` enfocados en `Exercises` y el nuevo servicio de sesiones.
4. Verificación manual: iniciar una sesión desde la nueva página, volver a un ejercicio, registrar una serie y confirmar que el `Set` queda con `WorkoutSessionId` informado; repetir en el mismo día y confirmar que no se crea una segunda sesión al añadir nuevas series si la regla elegida es reutilizar la sesión del día.
5. Verificación manual negativa: intentar registrar una serie sin sesión del día y confirmar que el sistema redirige a la página de inicio de sesión en lugar de crear sets huérfanos.

**Decisions**
- Incluido: chequeo de sesión activa antes de añadir set, página explícita para crear sesión, localización obligatoria, plan opcional, asociación automática del set a la sesión activa del día.
- Excluido por ahora: cierre manual de sesión, gestión de múltiples sesiones activas en un mismo día, preferencias persistentes de localización/plan por defecto.
- Decisión funcional: “sesión activa” significa cualquier `WorkoutSession` creada hoy; si existen varias, reutilizar la más reciente para mantener una regla determinista.
- Decisión arquitectónica: no crear automáticamente la sesión desde el formulario de set porque la localización es obligatoria y el usuario pidió una página específica para elegirla.

**Further Considerations**
1. Si más adelante quieres distinguir “sesión abierta” frente a “sesión del día”, convendrá permitir `EndTime` nulo o añadir un flag explícito `IsClosed`; eso evita que “activa” dependa solo de la fecha.
2. Si una sesión debe restringir qué ejercicios pueden registrar sets según el plan elegido, habrá que añadir una validación adicional en `DetailModel` o `SetService` para comprobar que el ejercicio pertenece al plan activo.
3. Si esperas varias sesiones en un mismo día, merece la pena definir desde ya cómo selecciona el usuario la sesión objetivo en vez de reutilizar automáticamente la última.
