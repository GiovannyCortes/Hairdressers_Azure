﻿using Microsoft.AspNetCore.Mvc;
using Hairdressers_Azure.Filters;
using System.Security.Claims;
using Hairdressers_Azure.Helpers;
using Hairdressers_Azure.Models;
using Hairdressers_Azure.Interfaces;

namespace Hairdressers_Azure.Controllers {
    public class UserController : Controller {

        private readonly IConfiguration _configuration;
        private IRepositoryHairdresser repo_hairdresser;

        public UserController(IRepositoryHairdresser repo_hairdresser, IConfiguration configuration) {
            this.repo_hairdresser = repo_hairdresser;
            _configuration = configuration;
        }

        [AuthorizeUsers]
        public async Task<IActionResult> ControlPanel() {
            User user = new User {
                UserId = int.Parse(HttpContext.User.FindFirst("ID").Value),
                Name = HttpContext.User.Identity.Name,
                LastName = HttpContext.User.FindFirst("LAST_NAME").Value,
                Email = HttpContext.User.FindFirst("EMAIL").Value,
                EmailConfirmed = bool.Parse(HttpContext.User.FindFirst("EMAIL_CONFIRMED").Value),
                Phone = HttpContext.User.FindFirst("PHONE").Value
            };

            List<Hairdresser> hairdressers = await repo_hairdresser.GetHairdressersAsync(user.UserId);
            ViewData["HAIRDRESSERS"] = hairdressers;

            return View(user);
        }

        [AuthorizeUsers]
        public async Task<IActionResult> UserHairdressers() {
            return View();
        }

        [AuthorizeUsers]
        public async Task<IActionResult> SendValidateEmail() {
            User user = new User {
                UserId = int.Parse(HttpContext.User.FindFirst("ID").Value),
                Name = HttpContext.User.Identity.Name,
                LastName = HttpContext.User.FindFirst("LAST_NAME").Value,
                Email = HttpContext.User.FindFirst("EMAIL").Value,
                EmailConfirmed = bool.Parse(HttpContext.User.FindFirst("EMAIL_CONFIRMED").Value),
                Phone = HttpContext.User.FindFirst("PHONE").Value
            };
            string token = repo_hairdresser.GenerateToken() + user.UserId;
            await repo_hairdresser.AssignTokenAsync(user.UserId, token);
            await new HelperEmailService(_configuration).SendTemplateVerificationEmailAsync(user.Email, user.Name + " " + user.LastName, token);
            return Json("OK");
        }

        public async Task<IActionResult> ValidateEmail(string token) {
            int user_id = int.Parse(token.Substring(50));
            User? user = await repo_hairdresser.FindUserAsync(user_id);
            if (user != null) {
                bool response = await repo_hairdresser.ValidateTokenAsync(user_id, token);
                if (response) { // Validamos el email
                    await repo_hairdresser.ValidateEmailAsync(user_id);
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
            User? user = await repo_hairdresser.FindUserAsync(int.Parse(HttpContext.User.FindFirst("ID").Value));
            return View(user);
        }

        [AuthorizeUsers]
        [HttpPost]
        public async Task<IActionResult> UpdateUser(User user) {
            bool existe = false;
            if (HttpContext.User.FindFirst("EMAIL").Value != user.Email) {
                existe = await repo_hairdresser.EmailExist(user.Email);
            }

            if (user != null && !existe) {
                await repo_hairdresser.UpdateUserAsync(user.UserId, user.Name, user.LastName, user.Phone, user.Email);
            }
            return RedirectToAction("LogOut", "Managed");
        }

    }
}