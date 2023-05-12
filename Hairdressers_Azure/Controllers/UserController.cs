using CutAndGo.Interfaces;
using CutAndGo.Models;
using Hairdressers_Azure.Filters;
using Hairdressers_Azure.Helpers;
using Hairdressers_Azure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Hairdressers_Azure.Controllers {
    public class UserController : Controller {
        
        private ServiceCutAndGo service;
        private readonly IConfiguration _configuration;

        public UserController(ServiceCutAndGo service, IConfiguration configuration) {
            this.service = service;
            _configuration = configuration;
        }

        [AuthorizeUsers]
        public async Task<IActionResult> ControlPanel() {
            ViewData["HAIRDRESSERS"] = await service.GetHairdressersAsync();
            return View();
        }

        /*[AuthorizeUsers]
        public async Task<IActionResult> SendValidateEmail() {
            int userId = int.Parse(HttpContext.User.FindFirst("ID").Value);
            string name = HttpContext.User.Identity.Name;
            string lastName = HttpContext.User.FindFirst("LAST_NAME").Value;
            string email = HttpContext.User.FindFirst("EMAIL").Value;
            string token = service.GenerateToken() + userId;

            await service.UserAssignTokenAsync(userId, token);
            await new HelperEmailService(_configuration).SendTemplateVerificationEmailAsync(email, name + " " + lastName, token);
            return Json("OK");
        }

        public async Task<IActionResult> ValidateEmail(string token) {
            int user_id = int.Parse(token.Substring(50));
            User? user = await service.FindUserAsync(user_id);
            if (user != null) {
                bool response = await service.ValidateTokenAsync(user_id, token);
                if (response) { // Validamos el email
                    await service.ValidateEmailAsync(user_id);
                }
                ViewData["RESPONSE"] = response ? 1 : 2; /// 1: Usuario encontrado y validado | 2: Usuario encontrado pero no validado
                return View(user);
            } else {
                ViewData["RESPONSE"] = 3; /// 3: Usuario NO encontrado
                return View();
            }
        }

        [AuthorizeUsers]
        public async Task<IActionResult> UpdateUser() {
            User? user = await service.FindUserAsync(int.Parse(HttpContext.User.FindFirst("ID").Value));
            return View(user);
        }

        [AuthorizeUsers]
        [HttpPost]
        public async Task<IActionResult> UpdateUser(User user) {
            bool existe = false;
            if (HttpContext.User.FindFirst("EMAIL").Value != user.Email) {
                existe = await service.EmailExist(user.Email);
            }

            if (user != null && !existe) {
                await service.UpdateUserAsync(user.UserId, user.Name, user.LastName, user.Phone, user.Email);
            }
            return RedirectToAction("LogOut", "Managed");
        }*/
        
    }
}