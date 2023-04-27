using Microsoft.AspNetCore.Mvc;
using Hairdressers_Azure.Filters;
using Hairdressers_Azure.Helpers;

namespace Hairdressers_Azure.Controllers {
    public class HairdresserController : Controller {
        /*
        private IRepositoryHairdresser repo_hairdresser;

        public HairdresserController(IRepositoryHairdresser repo_hairdresser) {
            this.repo_hairdresser = repo_hairdresser;
        }

        [AuthorizeUsers]
        public async Task<IActionResult> ControlPanel(string hid) {
            int hairdresser_id = int.Parse(hid);
            Hairdresser? hairdresser = await repo_hairdresser.FindHairdresserAsync(hairdresser_id);
            if (hairdresser != null) {
                List<Schedule> schedules = await repo_hairdresser.GetSchedulesAsync(hairdresser_id, true);
                ViewData["SCHEDULES"] = schedules;
                ViewData["FIRST_SCHEDULE"] = (schedules.Count > 0) ? schedules[0].Name : "";
                return View(hairdresser);
            } else {
                ViewData["ERROR_MESSAGE_TITLE"] = "Se ha producido un error inesperado";
                ViewData["ERROR_MESSAGE_SUBTITLE"] = "Peluquería no encontrada";
                return RedirectToAction("Error", "Managed");
            }
        }

        [AuthorizeUsers]
        public IActionResult CreateHairdresser() {
            return View();
        }

        [AuthorizeUsers] [ValidateAntiForgeryToken] [HttpPost]
        public async Task<IActionResult> CreateHairdresser(Hairdresser hairdresser, string schedules) {
            // Insertamos la nueva peluquería 
            int user_id = int.Parse(HttpContext.User.FindFirst("ID").Value);
            int newHId = await repo_hairdresser.InsertHairdresserAsync(hairdresser.Name, hairdresser.Phone, hairdresser.Address, hairdresser.PostalCode, user_id);

            // Insertamos el horario por defecto 'Horario General'
            int newSid = await repo_hairdresser.InsertScheduleAsync(newHId, "Horario General", true);

            // Recuperamos la lista de registros del horario
            List<Schedule_Row> schedules_rows = HelperJson.DeserializeObject<List<Schedule_Row>>(schedules);
            foreach (Schedule_Row r in schedules_rows) {
                await repo_hairdresser.InsertScheduleRowsAsync(newSid, r.Start, r.End, r.Monday, r.Tuesday, r.Wednesday, r.Thursday, r.Friday, r.Saturday, r.Sunday);
            }
            return RedirectToAction("ControlPanel", "User");
        }

        [AuthorizeUsers] [ValidateAntiForgeryToken] [HttpPost]
        public async Task<IActionResult> UpdateHairdresser(Hairdresser hairdresser) {
            await repo_hairdresser.UpdateHairdresserAsync(hairdresser.HairdresserId, hairdresser.Name, hairdresser.Phone, hairdresser.Address, hairdresser.PostalCode);
            return RedirectToAction("ControlPanel", "Hairdresser", new { hid = hairdresser.HairdresserId });
        }

        [AuthorizeUsers]
        public async Task<IActionResult> DeleteHairdresser(int hairdresser_id) {
            await repo_hairdresser.DeleteHairdresserAsync(hairdresser_id);
            return Json("/User/ControlPanel");
        }

        [AuthorizeUsers]
        public async Task<JsonResult> GetHairdresserSuggestions(string searchString) {
            List<Hairdresser> hairdressers = await repo_hairdresser.GetHairdressersByFilter(searchString);
            string sugerencias = HelperJson.SerializeObject(hairdressers);
            return Json(sugerencias);
        }

        [AuthorizeUsers]
        public async Task<IActionResult> Services(int hairdresserId, string hairdresserName) {
            List<Service> services = await repo_hairdresser.GetServicesByHairdresserAsync(hairdresserId);
            ViewData["HAIRDRESSER_ID"] = hairdresserId;
            ViewData["HAIRDRESSER_NAME"] = hairdresserName;
            return View(services);
        }

        [AuthorizeUsers]
        public async Task<IActionResult> AddService(int hairdresser_id, string name, string price, string time) {
            byte time_in_minutes = (byte)TimeSpan.Parse(time.Replace('.', ',')).TotalMinutes;
            int service_id = await repo_hairdresser.InsertServiceAsync(hairdresser_id, name, decimal.Parse(price.Replace('.', ',')), time_in_minutes);
            return Json(service_id);
        }

        [AuthorizeUsers]
        public async Task<IActionResult> RemoveService(int service_id) {
            await repo_hairdresser.DeleteServiceAsync(service_id);
            return Json("OK");
        }

        [AuthorizeUsers]
        public async Task<IActionResult> _ScheduleContainerPartial(int hairdresser_id = 0) {
            List<Schedule> schedules;
            if (hairdresser_id != 0) {
                schedules = await this.repo_hairdresser.GetSchedulesAsync(hairdresser_id, true);
                ViewData["FIRST_SCHEDULE"] = schedules[0].Name;
            } else {
                schedules = new List<Schedule>();
            }
            ViewData["HairdresserId"] = hairdresser_id;
            return PartialView("_ScheduleContainerPartial", schedules);
        }

        [AuthorizeUsers]
        public async Task<ActionResult> CreateSchedule(int hairdresser_id, string name) {
            await this.repo_hairdresser.InsertScheduleAsync(hairdresser_id, name, false);
            return Json("OK");
        }
        
        [AuthorizeUsers]
        public async Task<ActionResult> DeleteSchedule(int schedule_id) {
            int res = await this.repo_hairdresser.DeleteScheduleAsync(schedule_id);
            return Json(res);
        }

        [AuthorizeUsers]
        public async Task<ActionResult> ActivateSchedule(int hairdresser_id, int schedule_id) {
            await this.repo_hairdresser.ActivateScheduleAsync(hairdresser_id, schedule_id);
            return Json("OK");
        }

        [AuthorizeUsers]
        public async Task<ActionResult> AddScheduleRow(string apertura, string cierre, string daysText, int schedule_id) {
            TimeSpan start = new TimeSpan(int.Parse(apertura.Split(':')[0]), int.Parse(apertura.Split(':')[1]), 0);
            TimeSpan end = new TimeSpan(int.Parse(cierre.Split(':')[0]), int.Parse(cierre.Split(':')[1]), 0);
            Response res = await this.repo_hairdresser.InsertScheduleRowsAsync(schedule_id, start, end, daysText.Contains("L"), 
                                                                                                        daysText.Contains("M"), 
                                                                                                        daysText.Contains("X"), 
                                                                                                        daysText.Contains("J"), 
                                                                                                        daysText.Contains("V"), 
                                                                                                        daysText.Contains("S"), 
                                                                                                        daysText.Contains("D"));
            return Json(HelperJson.SerializeObject(res));
        }

        [AuthorizeUsers]
        public async Task<ActionResult> DeleteScheduleRow(int scheduleRow_id) {
            await repo_hairdresser.DeleteScheduleRowsAsync(scheduleRow_id);
            return Json("OK");
        }
        */
    }
}
