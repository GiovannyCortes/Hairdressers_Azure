using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using CutAndGo.Models;
using System.Security.Claims;
using Hairdressers_Azure.Services;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace Hairdressers_Azure.Controllers {
    public class ManagedController : Controller {
        
        private ServiceCutAndGo service;
        private ServiceStorageBlobs serviceBlob;

        public ManagedController(ServiceCutAndGo service, ServiceStorageBlobs serviceBlob) {
            this.service = service;
            this.serviceBlob = serviceBlob;
        }

        public IActionResult LogIn() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(string email, string password) {
            
            // EXTRACCIÓN DEL TOKEN
            string responseToken = await this.service.LogInAsync(email, password);
            dynamic responseObject = JsonConvert.DeserializeObject(responseToken);
            string token = responseObject.response;

            // EXTRACCIÓN DEL USUARIO DE DICHO TOKEN
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken parsedToken = tokenHandler.ReadJwtToken(token);

            IEnumerable<Claim> claims = parsedToken.Claims;
            Claim userDataClaim = claims.FirstOrDefault(c => c.Type == "UserData");
            User? user = null;
            if (userDataClaim != null) {
                string userDataJson = userDataClaim.Value;
                user = JsonConvert.DeserializeObject<User?>(userDataJson);
            }

            HttpContext.Session.SetString("TOKEN", token);

            if (user != null) { // Usuario encontrado, credenciales correctas
                await SignInUser(user); // Ejecutamos el LogIn del Usuario
                await ChargeImage(user.Image); // Cargamos la imagen del usuario para no sobrecargar el storage a peticiones

                string controller = TempData["controller"].ToString();
                string action = TempData["action"].ToString();

                return RedirectToAction(action, controller);
            } else {
                ViewData["VERIFICATION"] = "Credenciales incorrectas";
                return View();
            }
        }

        public async Task<IActionResult> LogOut() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("TOKEN");
            HttpContext.Session.Remove("IMAGE_USER");
            return RedirectToAction("Index", "Landing");
        }

        public IActionResult Registrer() {
            return View();
        }

        [HttpPost] [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrer(string Password, string Name, string LastName, string Phone, string Email, IFormFile file) {
            string extension = System.IO.Path.GetExtension(file.FileName);
            User? user = await this.service.InsertUserAsync(Name, LastName, Phone, Email, Password, extension);

            string blobName = "user_" + user.UserId + extension;
            using (Stream stream = file.OpenReadStream()) {
                await this.serviceBlob.UploadBlobAsync("users", blobName, stream);
            }

            if (user != null) {
                await SignInUser(user); // Ejecutamos el LogIn del nuevo Usuario registrado
                await ChargeImage(user.Image); // Cargamos la imagen del usuario para no sobrecargar el storage a peticiones
                return RedirectToAction("ControlPanel", "User"); // Le enviamos al Panel de Control
            }
            return View(""); // Si el registro falla, se mantiene la vista
        }

        public IActionResult AccesoDenegado() {
            return View();
        }

        public IActionResult Error() {
            return View();
        }

        private async Task ChargeImage(string blobName) {
            if (blobName != null && blobName != "") {
                BlobContainerClient blobContainerClient = await this.serviceBlob.GetContainerAsync("users");
                BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

                BlobSasBuilder sasBuilder = new BlobSasBuilder() {
                    BlobContainerName = "users",
                    BlobName = blobName,
                    Resource = "b",
                    StartsOn = DateTimeOffset.UtcNow,
                    ExpiresOn = DateTime.UtcNow.AddHours(1),
                };

                sasBuilder.SetPermissions(BlobSasPermissions.Read);
                var uri = blobClient.GenerateSasUri(sasBuilder);
                HttpContext.Session.SetString("IMAGE_USER", uri.ToString());
            }
        }

        private async Task SignInUser(User user) {
            ClaimsIdentity identity = new ClaimsIdentity(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    ClaimTypes.Name, ClaimTypes.Role
            );

            string admin = await this.service.UserIsAdminAsync(user.UserId) ? "ADMIN" : "CLIENT";

            Claim claimUserID = new Claim("ID", user.UserId.ToString());
            Claim claimUserName = new Claim(ClaimTypes.Name, user.Name);
            Claim claimUserLastName = new Claim("LAST_NAME", user.LastName);
            Claim claimUserEmail = new Claim("EMAIL", user.Email);
            Claim claimUserEmailConfirmed = new Claim("EMAIL_CONFIRMED", user.EmailConfirmed.ToString());
            Claim claimUserPhone = new Claim("PHONE", user.Phone);
            Claim claimUserImage = new Claim("IMAGE", user.Image);
            Claim claimUserRole = new Claim(ClaimTypes.Role, admin);

            identity.AddClaim(claimUserID);
            identity.AddClaim(claimUserName);
            identity.AddClaim(claimUserLastName);
            identity.AddClaim(claimUserEmail);
            identity.AddClaim(claimUserEmailConfirmed);
            identity.AddClaim(claimUserPhone);
            identity.AddClaim(claimUserImage);
            identity.AddClaim(claimUserRole);

            ClaimsPrincipal userPrincipal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                userPrincipal
            );
        }

    }
}
