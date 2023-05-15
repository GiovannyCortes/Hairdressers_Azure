using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Hairdressers_Azure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Obtener valor del secreto de Azure Key Vault para los Nuevos Blobs
string keyVaultUrl = builder.Configuration["KeyVault:VaultUri"];
SecretClient secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());
KeyVaultSecret azureKeys = await secretClient.GetSecretAsync("storagecutandgo");

BlobServiceClient blobServiceClient = new BlobServiceClient(azureKeys.Value);
    builder.Services.AddTransient<BlobServiceClient>(x => blobServiceClient);

// Old Blobs 
// string azureKeys = builder.Configuration.GetValue<string>("AzureKeys:StorageAccount");
// BlobServiceClient blobServiceClient = new BlobServiceClient(azureKeys);
// builder.Services.AddTransient<BlobServiceClient>(x => blobServiceClient);

    builder.Services.AddTransient<ServiceStorageBlobs>();

    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options => {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
    });

    builder.Services.AddAuthentication(options => {
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    }).AddCookie();

    builder.Services.AddHttpContextAccessor();

    builder.Services.AddAzureClients(factory => {
        factory.AddSecretClient(builder.Configuration.GetSection("KeyVault"));
    });

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
