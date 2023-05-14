using CutAndGo.Models;
using Microsoft.AspNetCore.Mvc;
using Hairdressers_Azure.Filters;
using Hairdressers_Azure.Helpers;
using Hairdressers_Azure.Services;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace Hairdressers_Azure.Controllers {
    public class HairdresserController : Controller {
        
        private ServiceCutAndGo service;
        private ServiceStorageBlobs serviceBlob;

        public HairdresserController(ServiceCutAndGo service, ServiceStorageBlobs serviceBlob) {
            this.service = service;
            this.serviceBlob = serviceBlob;
        }

        [AuthorizeUsers]
        public async Task<IActionResult> ControlPanel(string hid) {
            int hairdresser_id = int.Parse(hid);
            Hairdresser? hairdresser = await this.service.FindHairdresserAsync(hairdresser_id);
            if (hairdresser != null) {
                List<Schedule> schedules = await this.service.GetSchedulesAsync(hairdresser_id, true);
                hairdresser.Image = await this.GetHairdresserImage(hairdresser.Image);
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
        public async Task<IActionResult> CreateHairdresser(HairdresserRegister hairdresser, string schedules, IFormFile file) {
            // Insertamos la nueva peluquería 
            int user_id = int.Parse(HttpContext.User.FindFirst("ID").Value);
            string? extension = (file == null) ? null : System.IO.Path.GetExtension(file.FileName);
            int newHId = await this.service.InsertHairdresserAsync(hairdresser.Name, hairdresser.Phone, hairdresser.Address, hairdresser.PostalCode, extension, user_id);

            // Insertamos la imagen en el Container
            if (file != null) { // Si no se ha registrado una imagen de usuario no se ejecuta ningún guardado de BLOB
                string blobName = "hairdresser_" + newHId + extension;
                using (Stream stream = file.OpenReadStream()) {
                    await this.serviceBlob.UploadBlobAsync("hairdressers", blobName, stream);
                }
            }

            // Insertamos el horario por defecto 'Horario General'
            int newSid = await this.service.InsertScheduleAsync(newHId, "Horario General", true);

            // Recuperamos la lista de registros del horario
            List<Schedule_Row> schedules_rows = HelperJson.DeserializeObject<List<Schedule_Row>>(schedules);
            foreach (Schedule_Row r in schedules_rows) {
                await this.service.InsertScheduleRowsAsync(newSid, r.Start, r.End, r.Monday, r.Tuesday, r.Wednesday, r.Thursday, r.Friday, r.Saturday, r.Sunday);
            }
            return RedirectToAction("ControlPanel", "User");
        }

        [AuthorizeUsers] [ValidateAntiForgeryToken] [HttpPost]
        public async Task<IActionResult> UpdateHairdresser(HairdresserUpdates hairdresser, IFormFile file) {
            string? extension = (file == null) ? null : System.IO.Path.GetExtension(file.FileName);
            if (file != null) { // Si no se ha registrado una nueva imagen de usuario no se ejecuta cambio de Blob
                Hairdresser? oldHairdresser = await this.service.FindHairdresserAsync(hairdresser.HairdresserId);
                if (oldHairdresser != null && oldHairdresser.Image != null && oldHairdresser.Image != "") {
                    await this.serviceBlob.DeleteBlobAsync("hairdressers", oldHairdresser.Image); // Eliminamos el antiguo Blob
                }
                // Insertamos el nuevo Blob
                string blobName = "hairdresser_" + hairdresser.HairdresserId + extension;
                using (Stream stream = file.OpenReadStream()) {
                    await this.serviceBlob.UploadBlobAsync("hairdressers", blobName, stream);
                }
            }
            // Actualizamos la nueva peluquería
            await this.service.UpdateHairdresserAsync(hairdresser.HairdresserId, hairdresser.Name, hairdresser.Phone, hairdresser.Address, hairdresser.PostalCode, extension);
            return RedirectToAction("ControlPanel", "Hairdresser", new { hid = hairdresser.HairdresserId });
        }

        [AuthorizeUsers]
        public async Task<IActionResult> DeleteHairdresser(int hairdresser_id) {
            Hairdresser? removeHairdresser = await this.service.FindHairdresserAsync(hairdresser_id);
            if (removeHairdresser != null && removeHairdresser.Image != null && removeHairdresser.Image != "") {
                await this.serviceBlob.DeleteBlobAsync("hairdressers", removeHairdresser.Image); // Eliminamos el antiguo Blob
            }
            await this.service.DeleteHairdresserAsync(hairdresser_id);
            return Json("/User/ControlPanel");
        }

        [AuthorizeUsers]
        public async Task<JsonResult> GetHairdresserSuggestions(string searchString) {
            List<Hairdresser> hairdressers = await this.service.GetHairdressersByFilterNameAsync(searchString);
            string sugerencias = HelperJson.SerializeObject(hairdressers);
            return Json(sugerencias);
        }

        [AuthorizeUsers]
        public async Task<IActionResult> Services(int hairdresserId, string hairdresserName) {
            List<Service> services = await this.service.GetServicesByHairdresserAsync(hairdresserId);
            ViewData["HAIRDRESSER_ID"] = hairdresserId;
            ViewData["HAIRDRESSER_NAME"] = hairdresserName;
            return View(services);
        }

        [AuthorizeUsers]
        public async Task<IActionResult> AddService(int hairdresser_id, string name, string price, string time) {
            byte time_in_minutes = (byte)TimeSpan.Parse(time.Replace('.', ',')).TotalMinutes;
            int service_id = await this.service.InsertServiceAsync(hairdresser_id, name, decimal.Parse(price.Replace('.', ',')), time_in_minutes);
            return Json(service_id);
        }

        [AuthorizeUsers]
        public async Task<IActionResult> RemoveService(int service_id) {
            await this.service.DeleteServiceAsync(service_id);
            return Json("OK");
        }

        [AuthorizeUsers]
        public async Task<IActionResult> _ScheduleContainerPartial(int hairdresser_id = 0) {
            List<Schedule> schedules;
            if (hairdresser_id != 0) {
                schedules = await this.service.GetSchedulesAsync(hairdresser_id, true);
                ViewData["FIRST_SCHEDULE"] = schedules[0].Name;
            } else {
                schedules = new List<Schedule>();
            }
            ViewData["HairdresserId"] = hairdresser_id;
            return PartialView("_ScheduleContainerPartial", schedules);
        }

        [AuthorizeUsers]
        public async Task<ActionResult> CreateSchedule(int hairdresser_id, string name) {
            await this.service.InsertScheduleAsync(hairdresser_id, name, false);
            return Json("OK");
        }
        
        [AuthorizeUsers]
        public async Task<ActionResult> DeleteSchedule(int schedule_id) {
            int res = await this.service.DeleteScheduleAsync(schedule_id);
            return Json(res);
        }

        [AuthorizeUsers]
        public async Task<ActionResult> ActivateSchedule(int hairdresser_id, int schedule_id) {
            await this.service.ActivateScheduleAsync(hairdresser_id, schedule_id);
            return Json("OK");
        }

        [AuthorizeUsers]
        public async Task<ActionResult> AddScheduleRow(string apertura, string cierre, string daysText, int schedule_id) {
            TimeSpan start = new TimeSpan(int.Parse(apertura.Split(':')[0]), int.Parse(apertura.Split(':')[1]), 0);
            TimeSpan end = new TimeSpan(int.Parse(cierre.Split(':')[0]), int.Parse(cierre.Split(':')[1]), 0);
            int satisfactoryId = await this.service.InsertScheduleRowsAsync(schedule_id, start, end, daysText.Contains("L"), 
                                                                                                     daysText.Contains("M"), 
                                                                                                     daysText.Contains("X"), 
                                                                                                     daysText.Contains("J"), 
                                                                                                     daysText.Contains("V"), 
                                                                                                     daysText.Contains("S"), 
                                                                                                     daysText.Contains("D"));
            return Json(satisfactoryId);
        }

        [AuthorizeUsers]
        public async Task<ActionResult> DeleteScheduleRow(int scheduleRow_id) {
            await this.service.DeleteScheduleRowsAsync(scheduleRow_id);
            return Json("OK");
        }

        private async Task<string> GetHairdresserImage(string blobName) {
            if (blobName != null && blobName != "") {
                BlobContainerClient blobContainerClient = await this.serviceBlob.GetContainerAsync("hairdressers");
                BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

                BlobSasBuilder sasBuilder = new BlobSasBuilder() {
                    BlobContainerName = "hairdressers",
                    BlobName = blobName,
                    Resource = "b",
                    StartsOn = DateTimeOffset.UtcNow,
                    ExpiresOn = DateTime.UtcNow.AddHours(1),
                };

                sasBuilder.SetPermissions(BlobSasPermissions.Read);
                var uri = blobClient.GenerateSasUri(sasBuilder);
                return uri.ToString();
            }
            return blobName;
        }

    }
}
