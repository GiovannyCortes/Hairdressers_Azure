using Azure.Storage.Blobs;
using Hairdressers_Azure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Blobs
string azureKeys = builder.Configuration.GetValue<string>("AzureKeys:StorageAccount");
BlobServiceClient blobServiceClient = new BlobServiceClient(azureKeys);
builder.Services.AddTransient<BlobServiceClient>(x => blobServiceClient);

    builder.Services.AddTransient<ServiceStorageBlobs>();

    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options => {
        options.IdleTimeout = TimeSpan.FromMinutes(55);
    });

    builder.Services.AddAuthentication(options => {
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    }).AddCookie();

    builder.Services.AddHttpContextAccessor();

    builder.Services.AddTransient<ServiceCutAndGo>();

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
