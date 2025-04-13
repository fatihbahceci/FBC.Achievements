using FBC.Achievements.Components;
using FBC.Achievements.DBModels;
using Radzen;

var builder = WebApplication.CreateBuilder(args);
DB.MigrateDB();
//Logging  FBC

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//Radzen Components
builder.Services.AddRadzenComponents();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<DB>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
