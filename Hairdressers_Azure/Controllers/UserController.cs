using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using CutAndGo.Interfaces;
using CutAndGo.Models;
using Hairdressers_Azure.Filters;
using Hairdressers_Azure.Helpers;
using Hairdressers_Azure.Services;
using Microsoft.AspNetCore.Mvc;
using NugetHairdressersAzure.Models;

namespace Hairdressers_Azure.Controllers {
    public class UserController : Controller {
        
        private ServiceCutAndGo service;
        private ServiceStorageBlobs serviceBlob;
        private readonly IConfiguration _configuration;

        public UserController(ServiceCutAndGo service, IConfiguration configuration, ServiceStorageBlobs serviceBlob) {
            this.service = service;
            this.serviceBlob = serviceBlob;
            this._configuration = configuration;
        }

        [AuthorizeUsers]
        public async Task<IActionResult> ControlPanel() {
            ViewData["HAIRDRESSERS"] = await this.service.GetHairdressersAsync();
            return View();
        }

        [AuthorizeUsers]
        public async Task<IActionResult> SendValidateEmail() {
            int userId = int.Parse(HttpContext.User.FindFirst("ID").Value);
            string name = HttpContext.User.Identity.Name;
            string lastName = HttpContext.User.FindFirst("LAST_NAME").Value;
            string email = HttpContext.User.FindFirst("EMAIL").Value;
            string token = await this.service.GenerateToken() + userId;

            await service.UserAssignTokenAsync(userId, token);
            await new HelperEmailService(this._configuration).SendTemplateVerificationEmailAsync(email, name + " " + lastName, token);
            return Json("OK");
        }

        public async Task<IActionResult> ValidateEmail(string token) {
            int user_id = int.Parse(token.Substring(50));
            int response  = await service.ValidateEmailAsync(user_id, token);

            ViewData["RESPONSE"] = (response == user_id) ? 1 : 2; /// 1: Usuario encontrado y validado | 2: Usuario encontrado pero no validado
            return View();
        }

        [AuthorizeUsers]
        public async Task<IActionResult> UpdateUser() {
            User? user = await service.FindUserAsync(int.Parse(HttpContext.User.FindFirst("ID").Value));
            return View(user);
        }

        [AuthorizeUsers] [HttpPost]
        public async Task<IActionResult> UpdateUser(UserUpdates user, IFormFile file) {
            // Comprobamos si existe un nuevo email con los claims del usuario
            bool existe = false; // Por defecto el email no está duplicado y no se han registrado cambios
            if (HttpContext.User.FindFirst("EMAIL").Value != user.Email) {
                existe = await this.service.EmailExistAsync(user.Email); // Verificamos el email
            }

            if (user != null && !existe) { // Si el usuario ha sido encontrado y no existe duplicidad de email
                string? extension = (file == null) ? null : System.IO.Path.GetExtension(file.FileName);
                if (file != null) { // Si no se ha registrado una nueva imagen de usuario no se ejecuta cambio de Blob
                    User? oldUser = await this.service.FindUserAsync(user.UserId);
                    if (oldUser != null && oldUser.Image != null && oldUser.Image != "") { 
                        await this.serviceBlob.DeleteBlobAsync("users", oldUser.Image); // Eliminamos el antiguo Blob
                    }
                    // Insertamos el nuevo Blob
                    string blobName = "user_" + user.UserId + extension;
                    using (Stream stream = file.OpenReadStream()) {
                        await this.serviceBlob.UploadBlobAsync("users", blobName, stream);
                    }
                }
                // Actualizamos el usuario
                await this.service.UpdateUserAsync(user.UserId, user.Name, user.LastName, user.Phone, user.Email, extension);
            }
            return RedirectToAction("LogOut", "Managed", new { toLogIn = true });
        }
        
    }
}