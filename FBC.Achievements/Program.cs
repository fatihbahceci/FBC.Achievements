using FBC.Achievements;
using FBC.Achievements.Components;
using FBC.Achievements.DBModels;
using FBC.Achievements.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using Radzen;

var builder = WebApplication.CreateBuilder(args);
DB.MigrateDB();
//Logging  FBC

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<IStringLocalizer>(provider =>
{
    var env = provider.GetRequiredService<IHostEnvironment>();
    var filePath = Path.Combine(AppContext.BaseDirectory, "locales.json");
    return new JsonStringLocalizer(filePath);
});

//Radzen Components
builder.Services.AddRadzenComponents();
#region Unable to find the required 'IAuthenticationService' service (When logout and refresh the page)
builder.Services.AddAuthentication("FakeScheme")
    .AddScheme<AuthenticationSchemeOptions, FakeAuthenticationHandler>("FakeScheme", options => { });
builder.Services.AddAuthorization();
#endregion

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

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
app.UseRouting();
app.UseAntiforgery();
#region Unable to find the required 'IAuthenticationService' service (When logout and refresh the page)
app.UseAuthentication();
app.UseAuthorization();
#endregion
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();