using Hairdressers_Azure.Data;
using Hairdressers_Azure.Interfaces;
using Hairdressers_Azure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

string connectionstring = builder.Configuration.GetConnectionString("SqlHairdressersTajamar");

    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options => {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
    });

    builder.Services.AddTransient<IRepositoryHairdresser, RepositoryHairdresser>();
    builder.Services.AddDbContext<HairdressersContext> (
        options => options.UseSqlServer(connectionstring)
    );

    builder.Services.AddAuthentication(options => {
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    }).AddCookie();

    builder.Services.AddAntiforgery();
    builder.Services.AddControllersWithViews(options => {
        options.EnableEndpointRouting = false;
    }).AddSessionStateTempDataProvider();

    builder.Configuration.AddJsonFile("appsettings.json");

var app = builder.Build();
    app.UseStaticFiles();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseSession();
    app.UseMvc(routes => {
        routes.MapRoute(
            name: "default",
            template: "{controller=Landing}/{action=Index}"
        );
    });

app.Run();