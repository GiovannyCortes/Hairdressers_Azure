using Hairdressers_Azure.Helpers;
using Hairdressers_Azure.Interfaces;
using Hairdressers_Azure.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hairdressers_Azure.Controllers {
    public class LandingController : Controller {

        public IActionResult Index() {
            Schedule_Row prueba = new Schedule_Row { 
                Start = new TimeSpan(8,0,0),
                End = new TimeSpan(14,30,0),
                Monday = true,
                Tuesday = true,
                Wednesday = true,
                Thursday = true,
                Friday = true,
                Saturday = false,
                Sunday = false,
                ScheduleRowId = 0,
                ScheduleId = 0
            };
            List<Schedule_Row> cosa = new List<Schedule_Row>();
            cosa.Add(prueba);
            ViewData["PRUEBA"] = HelperCalendar.GetBussinesHours(cosa);
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
