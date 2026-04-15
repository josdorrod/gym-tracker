using GymTracker.Data;
using GymTracker.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<GymTrackerDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register application services.
builder.Services.AddScoped<IExerciseService, ExerciseService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<ISetService, SetService>();
builder.Services.AddScoped<IDbExceptionHelper, SqliteDbExceptionHelper>();
builder.Services.AddScoped<IPlanService, PlanService>();

var app = builder.Build();

// Auto-migrate database on startup.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GymTrackerDbContext>();
    await db.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
