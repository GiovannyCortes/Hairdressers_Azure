﻿using Microsoft.AspNetCore.Mvc;

namespace Hairdressers_Azure.Controllers {
    public class LandingController : Controller {

        public IActionResult Index() {
            return View();
        }

        public IActionResult Help() {
            return View();
        }

        public IActionResult PrivatePolicy() {
            return View();
        }

        public IActionResult UseTerms() {
            return View();
        }

    }
}
