using CutAndGo.Models;
using Newtonsoft.Json;
using System.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using static CutAndGo.Interfaces.IRepositoryHairdresser;

namespace Hairdressers_Azure.Services {
    public class ServiceCutAndGo {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private MediaTypeWithQualityHeaderValue Header;
        private string UrlApi;

        public ServiceCutAndGo(IConfiguration configuration, IHttpContextAccessor httpContextAccessor) {
            this.UrlApi = configuration.GetValue<string>("ApiUrls:ApiCutAndGo");
            this.Header = new MediaTypeWithQualityHeaderValue("application/json");
            this._httpContextAccessor = httpContextAccessor;
        }

        #region GENERAL
        private async Task<T> CallApiAsync<T>(string request) {
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                HttpResponseMessage response = await client.GetAsync(request);
                return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<T>() : default(T);
            }
        }

        private async Task<HttpStatusCode> InsertApiAsync<T>(string request, T objeto) {
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                string json = JsonConvert.SerializeObject(objeto);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(request, content);
                return response.StatusCode;
            }
        }

        private async Task<HttpStatusCode> UpdateApiAsync<T>(string request, T objeto) {
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                string json = JsonConvert.SerializeObject(objeto);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(request, content);
                return response.StatusCode;
            }
        }

        // Se supone que en el request ya va el id. Ejemplo: http:/localhost/api/deletealgo/17
        private async Task<HttpStatusCode> DeleteApiAsync(string request) {
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();

                HttpResponseMessage response = await client.DeleteAsync(request);
                return response.StatusCode;
            }
        }
        #endregion

        #region GENERAL TOKEN
        private async Task<T> CallApiAsync<T>(string request, string token) {
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                HttpResponseMessage response = await client.GetAsync(request);
                return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<T>() : default(T);
            }
        }

        private async Task<string?> InsertApiAsync<T>(string request, T objeto, string token) {
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string json = JsonConvert.SerializeObject(objeto);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode) {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                } else {
                    return null;
                }
            }
        }

        private async Task<HttpStatusCode> UpdateApiAsync<T>(string request, T objeto, string token) {
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                string json = JsonConvert.SerializeObject(objeto);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(request, content);
                return response.StatusCode;
            }
        }

        // Se supone que en el request ya va el id. Ejemplo: http:/localhost/api/deletealgo/17
        private async Task<HttpStatusCode> DeleteApiAsync(string request, string token) {
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                HttpResponseMessage response = await client.DeleteAsync(request);
                return response.StatusCode;
            }
        }
        #endregion


        public Task<User?> LogInAsync(string email, string password) {
            throw new NotImplementedException();
        }

        #region TOKENS
        public async Task<string> GenerateToken() {
            string request = "/api/tokens/GenerateToken";
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<string>(request, contextToken);
        }

        public async Task<bool> UserAssignTokenAsync(int user_id, string token) {
            string request = "/api/tokens/UserAssignTokenAsync/" + user_id + "/" + token;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            string responseContent = await this.InsertApiAsync<string>(request, null, contextToken);
            return JsonConvert.DeserializeObject<bool>(responseContent);
        }

        public async Task<bool> UserValidateTokenAsync(int user_id, string token) {
            string request = "/api/tokens/UserValidateTokenAsync/" + user_id + "/" + token;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<bool>(request, contextToken);
        }

        public async Task<bool> HairdresserValidateTokenAsync(int hairdresser_id, string token) {
            string request = "/api/tokens/HairdresserValidateTokenAsync/" + hairdresser_id + "/" + token;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<bool>(request, contextToken);
        }
        #endregion

        #region ADMINS
        public async Task<Admin?> FindAdminAsync(int hairdresser_id, int user_id) {
            string request = "/api/admins/findadmin/" + hairdresser_id + "/" + user_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<Admin?>(request, contextToken);
        }

        public async Task<List<Admin>> GetAdminsAsync(int hairdresser_id) {
            string request = "/api/admins/getadmins/" + hairdresser_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<List<Admin>>(request, contextToken);
        }

        public async Task<bool> AdminExistAsync(int hairdresser_id, int user_id) {
            string request = "/api/admins/adminexist/" + hairdresser_id + "/" + user_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<bool>(request, contextToken);
        }

        public async Task<bool> CompareAdminRoleAsync(int hairdresser_id, int user_id_other) {
            string request = "/api/admins/compareadminrole/" + hairdresser_id + "/" + user_id_other;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<bool>(request, contextToken);
        }

        public async Task InsertAdminAsync(int hairdresser_id, int user_id, AdminRole role) {
            string request = "/api/admins/create";
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");

            Admin admin = new Admin {
                HairdresserId = hairdresser_id,
                UserId = user_id,
                Role = role
            };

            await this.InsertApiAsync<Admin>(request, admin, contextToken);
            //string responseContent = await this.InsertApiAsync<Admin>(request, admin, contextToken);
            //return JsonConvert.DeserializeObject<int>(responseContent);
        }

        public async Task UpdateAdminAsync(int hairdresser_id, int user_id, AdminRole role) {
            string request = "/api/admins/update";
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");

            Admin admin = new Admin {
                HairdresserId = hairdresser_id,
                UserId = user_id,
                Role = role
            };

            await this.UpdateApiAsync<Admin>(request, admin, contextToken);
        }

        public async Task DeleteAdminAsync(int hairdresser_id, int user_id_other) {
            string request = "/api/admins/delete/" + hairdresser_id + "/" + user_id_other;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.DeleteApiAsync(request, contextToken);
        }
        #endregion

        #region USERS
        public async Task<bool> UserIsAdminAsync(int user_id) {
            string request = "/api/users/userIsAdmin/" + user_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<bool>(request, contextToken);
        }

        public async Task<bool> EmailExistAsync(string email) {
            string request = "/api/users/emailexist/" + email;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<bool>(request, contextToken);
        }

        public async Task<User?> FindUserAsync(int user_id) {
            string request = "/api/users/finduser/" + user_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<User?>(request, contextToken);
        }

        public async Task<User?> InsertUserAsync(string name, string lastname, string phone, string email, string password, string image_extension) {
            string request = "/api/users/create";
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");

            UserRegister user = new UserRegister {
                Name = name,
                LastName = lastname,
                Phone = phone,
                Email = email,
                Password = password,
                ImageExtension = image_extension
            };

            string responseContent = await this.InsertApiAsync<UserRegister>(request, user, contextToken);
            return JsonConvert.DeserializeObject<User?>(responseContent);
        }

        public async Task UpdateUserAsync(int user_id, string name, string lastname, string phone, string email, string image_extension) {
            string request = "/api/users/update";
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");

            UserUpdates user = new UserUpdates {
                UserId = user_id,
                Name = name,
                LastName = lastname,
                Phone = phone,
                Email = email,
                ImageExtension = image_extension
            };

            await this.InsertApiAsync<UserUpdates>(request, user, contextToken);
        }

        public async Task DeleteUserAsync(int user_id) {
            string request = "/api/users/delete/" + user_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.DeleteApiAsync(request, contextToken);
        }

        public async Task ValidateEmailAsync(int user_id) {
            string request = "/api/users/ValidateEmail/" + user_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.UpdateApiAsync<User>(request, null, contextToken);
        }
        #endregion
        /*
        #region HAIRDRESSERS
        public async Task<Hairdresser?> FindHairdresserAsync(int hairdresser_id) {
            return await this.context.Hairdressers.FirstOrDefaultAsync(h => h.HairdresserId == hairdresser_id);
        }

        public async Task<List<string>> GetHairdresserEmailsAsync(int hairdresser_id) {
            var emails = from usuario in context.Users
                         join admin in context.Admins on usuario.UserId equals admin.UserId
                         where admin.HairdresserId == hairdresser_id
                         select usuario.Email;
            return await emails.ToListAsync();
        }

        public async Task<List<Hairdresser>> GetHairdressersAsync() {
            string request = "/api/hairdressers/gethairdressers";
            return await this.CallApiAsync<List<Hairdresser>>(request);
        }

        public async Task<List<Hairdresser>> GetHairdressersByUserAsync(int user_id) {
            var query = this.context.Hairdressers
                        .Join(
                            context.Admins,
                            hairdresser => hairdresser.HairdresserId,
                            admin => admin.HairdresserId,
                            (hairdresser, admin) => new { Hairdresser = hairdresser, Admin = admin }
                        )
                        .Join(
                            context.Users,
                            admin => admin.Admin.UserId,
                            user => user.UserId,
                            (admin, user) => new { admin.Hairdresser, User = user }
                        )
                        .Where(x => x.User.UserId == user_id).Select(x => x.Hairdresser);
            return await query.ToListAsync();
        }

        public async Task<List<Hairdresser>> GetHairdressersByFilterNameAsync(string filterName) {
            var query = from data in this.context.Hairdressers
                        where data.Name.ToLower().Contains(filterName.ToLower())
                        select data;
            return await query.ToListAsync();
        }

        public async Task<Response> InsertHairdresserAsync(string name, string phone, string address, int postal_code, string image_extension, int user_id) {
            var newid = await this.context.Hairdressers.AnyAsync() ? await this.context.Hairdressers.MaxAsync(s => s.HairdresserId) + 1 : 1;
            Hairdresser hairdresser = new Hairdresser {
                HairdresserId = newid,
                Name = name,
                Phone = phone,
                Address = address,
                PostalCode = postal_code,
                Image = "hairdresser_" + newid + image_extension,
                Token = GenerateToken()
            };
            this.context.Hairdressers.Add(hairdresser);
            int record = await this.context.SaveChangesAsync();
            Response response = await this.InsertAdminAsync(newid, user_id, AdminRole.Propietario);
            return (record > 0 && response.ResponseCode == (int)ResponseCodes.OK) ?
                    new Response { ResponseCode = (int)ResponseCodes.OK, SatisfactoryId = newid } :
                    new Response { ResponseCode = (int)ResponseCodes.Failed, ErrorCode = (int)ResponseErrorCodes.GeneralError };
        }

        public async Task<Response> UpdateHairdresserAsync(int hairdresser_id, string name, string phone, string address, int postal_code, string image_extension) {
            Hairdresser? hairdresser = await this.FindHairdresserAsync(hairdresser_id);
            int record = 0;
            if (hairdresser != null) {
                hairdresser.Name = name;
                hairdresser.Phone = phone;
                hairdresser.Address = address;
                hairdresser.PostalCode = postal_code;
                hairdresser.Image = "hairdresser_" + hairdresser_id + image_extension;
                record = await this.context.SaveChangesAsync();
            }
            return (record > 0) ? new Response { ResponseCode = (int)ResponseCodes.OK, SatisfactoryId = hairdresser_id } :
                                  new Response {
                                      ResponseCode = (int)ResponseCodes.Failed,
                                      ErrorCode = (int)ResponseErrorCodes.RecordNotFound,
                                      ErrorMessage = "Peluquería no encontrada"
                                  };
        }

        public async Task<Response> DeleteHairdresserAsync(int hairdresser_id) {
            Hairdresser? hairdresser = await this.FindHairdresserAsync(hairdresser_id);
            if (hairdresser != null) {
                // 1 - Borrar los registros de Admin
                List<Admin> admins = await this.GetAdminsAsync(hairdresser_id);
                foreach (Admin admin in admins) {
                    this.context.Admins.Remove(admin);
                }
                await this.context.SaveChangesAsync();

                // 2 - Borrar los registros de Horario
                List<Schedule> schedules = await this.GetSchedulesAsync(hairdresser_id, true);
                foreach (Schedule schedule in schedules) {
                    foreach (Schedule_Row schedule_Row in schedule.ScheduleRows) {
                        this.context.Schedule_Rows.Remove(schedule_Row);
                    }
                    await this.context.SaveChangesAsync();
                    this.context.Schedules.Remove(schedule);
                }
                await this.context.SaveChangesAsync();

                // 3 - Borrar los registros de citas y sus relaciones con servicios
                List<Appointment> appointments = await this.GetAppointmentsByHairdresserAsync(hairdresser_id);
                foreach (Appointment appointment in appointments) {
                    List<Appointment_Service> appointment_Services = await this.GetAppointmentServicesAsync(appointment.AppointmentId);
                    foreach (Appointment_Service appointment_Service in appointment_Services) {
                        this.context.AppointmentServices.Remove(appointment_Service);
                    }
                    await this.context.SaveChangesAsync();
                    this.context.Appointments.Remove(appointment);
                }
                await this.context.SaveChangesAsync();

                // 4 - Borrar los registros de servicios
                List<Service> services = await this.GetServicesByHairdresserAsync(hairdresser_id);
                foreach (Service service in services) {
                    this.context.Services.Remove(service);
                }
                await this.context.SaveChangesAsync();

                // 5 - Eliminar la peluquería
                this.context.Hairdressers.Remove(hairdresser);
                int record = await this.context.SaveChangesAsync();

                return (record > 0) ? new Response { SatisfactoryId = hairdresser_id, ResponseCode = (int)ResponseCodes.OK } :
                                      new Response {
                                          ResponseCode = (int)ResponseCodes.Failed,
                                          ErrorCode = (int)ResponseErrorCodes.GeneralError,
                                          ErrorMessage = "Error inesperado. La peluquería no ha podido ser eliminada"
                                      };
            } else {
                return new Response {
                    ResponseCode = (int)ResponseCodes.Failed,
                    ErrorCode = (int)ResponseErrorCodes.RecordNotFound,
                    ErrorMessage = "Peluquería no encontrada"
                };
            }
        }
        #endregion

        #region SCHEDULES
        public async Task<Schedule?> FindScheduleAsync(int schedule_id, bool getRows) {
            Schedule? schedule = await this.context.Schedules.FirstOrDefaultAsync(s => s.ScheduleId == schedule_id);
            if (schedule != null && getRows) {
                List<Schedule_Row> schedule_rows = await this.GetScheduleRowsAsync(schedule.ScheduleId);
                foreach (Schedule_Row row in schedule_rows) {
                    schedule.ScheduleRows.Add(row);
                }
            }
            return schedule;
        }

        public async Task<Schedule?> FindActiveScheduleAsync(int hairdresser_id, bool getRows) {
            return await this.context.Schedules.FirstOrDefaultAsync(s => s.HairdresserId == hairdresser_id && s.Active == true);
        }

        public async Task<List<string>> GetNameSchedulesAsync(int hairdresser_id) {
            var query = from data in context.Schedules
                        where data.HairdresserId == hairdresser_id
                        select data.Name;
            return await query.ToListAsync();
        }

        public async Task<List<Schedule>> GetSchedulesAsync(int hairdresser_id, bool getRows) {
            List<Schedule> schedules = await this.context.Schedules.Where(s => s.HairdresserId == hairdresser_id).ToListAsync();
            if (getRows) {
                foreach (Schedule sch in schedules) {
                    List<Schedule_Row> schedule_rows = await this.GetScheduleRowsAsync(sch.ScheduleId);
                    foreach (Schedule_Row row in schedule_rows) {
                        sch.ScheduleRows.Add(row);
                    }
                }
            }
            return schedules;
        }

        public async Task<Response> InsertScheduleAsync(int hairdresser_id, string name, bool active) {
            List<Schedule> schedules = await this.GetSchedulesAsync(hairdresser_id, false);
            bool duplicado = false;
            foreach (Schedule sch in schedules) {
                if (sch.Name.ToLower() == name.ToLower()) { duplicado = true; }
            }

            if (!duplicado) {
                if (active) { // Se pretende activar el nuevo horario
                    foreach (Schedule sch in schedules) {
                        if (sch.Active) { sch.Active = false; }
                    }
                }

                var newid = await this.context.Schedules.AnyAsync() ? await this.context.Schedules.MaxAsync(s => s.ScheduleId) + 1 : 1;
                Schedule schedule = new Schedule {
                    ScheduleId = newid,
                    HairdresserId = hairdresser_id,
                    Name = name,
                    Active = active
                };

                this.context.Schedules.Add(schedule);
                await this.context.SaveChangesAsync();
                return new Response { SatisfactoryId = newid, ResponseCode = (int)ResponseCodes.OK };
            } else {
                return new Response {
                    ResponseCode = (int)ResponseCodes.Failed,
                    ErrorCode = (int)ResponseErrorCodes.Duplicate,
                    ErrorMessage = "Ya existe un horario con el mismo nombre"
                };
            }
        }

        public async Task<Response> UpdateScheduleAsync(int schedule_id, int hairdresser_id, string name, bool active) {
            Schedule? schedule = await this.FindScheduleAsync(schedule_id, false);
            int record = 0;
            if (schedule != null) {
                schedule.Name = name;
                schedule.Active = active;

                if (active) {
                    List<Schedule> schedules = await GetSchedulesAsync(hairdresser_id, false);
                    foreach (Schedule sch in schedules) {
                        if (sch.ScheduleId != schedule_id && sch.Active) { sch.Active = false; }
                    }
                }

                record = await this.context.SaveChangesAsync();
            }
            return (record > 0) ? new Response { ResponseCode = (int)ResponseCodes.OK, SatisfactoryId = schedule_id } :
                                  new Response {
                                      ResponseCode = (int)ResponseCodes.Failed,
                                      ErrorCode = (int)ResponseErrorCodes.RecordNotFound,
                                      ErrorMessage = "Horario no encontrado"
                                  };
        }

        public async Task<Response> DeleteScheduleAsync(int schedule_id) {
            Schedule? schedule = await this.FindScheduleAsync(schedule_id, true);
            if (schedule != null) {
                List<Schedule> schedules = await GetSchedulesAsync(schedule.HairdresserId, false);
                if (schedules.Count > 1) {
                    if (schedule.ScheduleRows != null && schedule.ScheduleRows.Count > 0) {
                        foreach (Schedule_Row srow in schedule.ScheduleRows) {
                            this.context.Schedule_Rows.Remove(srow);
                            await this.context.SaveChangesAsync();
                        }
                    }
                    this.context.Schedules.Remove(schedule);
                    await this.context.SaveChangesAsync();
                    return new Response { ResponseCode = (int)ResponseCodes.OK };
                } else {
                    return new Response {
                        ResponseCode = (int)ResponseCodes.Failed,
                        ErrorCode = (int)ResponseErrorCodes.GeneralError,
                        ErrorMessage = "No puede haber peluquerías sin horario. Este es el último horario de la peluquería"
                    };
                }
            } else {
                return new Response {
                    ResponseCode = (int)ResponseCodes.Failed,
                    ErrorCode = (int)ResponseErrorCodes.RecordNotFound,
                    ErrorMessage = "Horario no encontrado"
                };
            }
        }

        public async Task<Response> ActivateScheduleAsync(int hairdresser_id, int schedule_id) {
            Schedule? schedule = await this.FindScheduleAsync(schedule_id, false);
            int record = 0;
            if (schedule != null) {
                List<Schedule> schedules = await GetSchedulesAsync(hairdresser_id, false);
                foreach (Schedule sch in schedules) {
                    if (sch.Active) { sch.Active = false; }
                }
                schedule.Active = true;
                record = await this.context.SaveChangesAsync();
            }
            return (record > 0) ? new Response { ResponseCode = (int)ResponseCodes.OK, SatisfactoryId = schedule_id } :
                                  new Response {
                                      ResponseCode = (int)ResponseCodes.Failed,
                                      ErrorCode = (int)ResponseErrorCodes.RecordNotFound,
                                      ErrorMessage = "Horario no encontrado"
                                  };
        }
        #endregion

        #region SCHEDULE_ROWS
        public async Task<Schedule_Row?> FindScheduleRowAsync(int schedule_row_id) {
            return await this.context.Schedule_Rows.FirstOrDefaultAsync(s => s.ScheduleRowId == schedule_row_id);
        }

        public async Task<List<Schedule_Row>> GetScheduleRowsAsync(int schedule_id) {
            return await this.context.Schedule_Rows.Where(s => s.ScheduleId == schedule_id).ToListAsync();
        }

        public async Task<List<Schedule_Row>> GetActiveScheduleRowsAsync(int hairdresser_id) {
            Schedule? schedule = await this.FindActiveScheduleAsync(hairdresser_id, false);
            if (schedule != null) {
                return await this.context.Schedule_Rows.Where(s => s.ScheduleId == schedule.ScheduleId).ToListAsync();
            } else {
                return new List<Schedule_Row>();
            }
        }

        public async Task<Response> InsertScheduleRowsAsync(int schid, TimeSpan start, TimeSpan end, bool mon, bool tue, bool wed, bool thu, bool fri, bool sat, bool sun) {
            var newid = await this.context.Schedule_Rows.AnyAsync() ? await this.context.Schedule_Rows.MaxAsync(s => s.ScheduleRowId) + 1 : 1;
            Schedule_Row schedule_row = new Schedule_Row {
                ScheduleRowId = newid,
                ScheduleId = schid,
                Start = start,
                End = end,
                Monday = mon,
                Tuesday = tue,
                Wednesday = wed,
                Thursday = thu,
                Friday = fri,
                Saturday = sat,
                Sunday = sun
            };

            Response response = await this.ValidateScheduleRowAsync(schedule_row);
            if (response.ResponseCode == (int)ResponseCodes.OK) {
                this.context.Schedule_Rows.Add(schedule_row);
                await this.context.SaveChangesAsync();
                return new Response { ResponseCode = (int)ResponseCodes.OK, SatisfactoryId = newid };
            } else {
                return response;
            }
        }

        public async Task<Response> ValidateScheduleRowAsync(Schedule_Row schedule_row) {
            int schrow_compare = TimeSpan.Compare(schedule_row.Start, schedule_row.End);
            if (schrow_compare == 0 || schrow_compare == 1) { // Validación de rango
                return new Response {
                    ResponseCode = (int)ResponseCodes.Failed,
                    ErrorCode = (int)ResponseErrorCodes.IncorrectRange,
                    ErrorMessage = "El inicio y el final del registro no conforman un rango válido"
                };
            }

            List<Schedule_Row> rows = await GetScheduleRowsAsync(schedule_row.ScheduleId);
            foreach (Schedule_Row row in rows) {
                if (
                    row.ScheduleRowId != schedule_row.ScheduleRowId &&
                    row.ScheduleId == schedule_row.ScheduleId &&
                    row.Start == schedule_row.Start &&
                    row.End == schedule_row.End &&
                    row.Monday == schedule_row.Monday &&
                    row.Tuesday == schedule_row.Tuesday &&
                    row.Wednesday == schedule_row.Wednesday &&
                    row.Thursday == schedule_row.Thursday &&
                    row.Friday == schedule_row.Friday &&
                    row.Saturday == schedule_row.Saturday &&
                    row.Sunday == schedule_row.Sunday
                ) {
                    return new Response {
                        ResponseCode = (int)ResponseCodes.Failed,
                        ErrorCode = (int)ResponseErrorCodes.Duplicate,
                        ErrorMessage = "Ya existe un registro con las mismas características"
                    };
                }

                if (
                    row.Monday & row.Monday == schedule_row.Monday |
                    row.Tuesday & row.Tuesday == schedule_row.Tuesday |
                    row.Wednesday & row.Wednesday == schedule_row.Wednesday |
                    row.Thursday & row.Thursday == schedule_row.Thursday |
                    row.Friday & row.Friday == schedule_row.Friday |
                    row.Saturday & row.Saturday == schedule_row.Saturday |
                    row.Sunday & row.Sunday == schedule_row.Sunday
                ) { // SOLO COMPROBAREMOS EL SOLAPAMIENTO DE TIEMPOS SI HAY COMO MÍNIMO UN DÍA COINCIDENTE
                    int start_compare_start = TimeSpan.Compare(schedule_row.Start, row.Start);
                    int start_compare_end = TimeSpan.Compare(schedule_row.Start, row.End);
                    int end_compare_start = TimeSpan.Compare(schedule_row.End, row.Start);
                    int end_compare_end = TimeSpan.Compare(schedule_row.End, row.End);

                    if (
                        row.ScheduleRowId != schedule_row.ScheduleRowId && (
                            start_compare_start == 0 ||                                                     // Los inicios igual al rango
                            start_compare_start == 1 && start_compare_end == -1 ||                          // El inicio está entre los rangos
                            start_compare_start == 1 && (end_compare_end == 0 || end_compare_end == -1) ||  // Inicio correcto, final menor o igual al rango
                            start_compare_start == -1 && end_compare_start == 1 && end_compare_end == -1 || // Inicio correcto, final entre rangos
                            start_compare_start == -1 && (end_compare_end == 0 || end_compare_end == 1)     // Inicio correcto, final superior al rango
                        )
                    ) {
                        return new Response {
                            ResponseCode = (int)ResponseCodes.Failed,
                            ErrorCode = (int)ResponseErrorCodes.OverwriteRange,
                            ErrorMessage = "Rango sobreescrito. Se ha producido un solapamiento"
                        };
                    }
                }

            }
            return new Response { ResponseCode = (int)ResponseCodes.OK };
        }

        public async Task<Response> DeleteScheduleRowsAsync(int schedule_row_id) {
            Schedule_Row? schedule_row = await this.FindScheduleRowAsync(schedule_row_id);
            int record = 0;
            if (schedule_row != null) {
                this.context.Schedule_Rows.Remove(schedule_row);
                record = await this.context.SaveChangesAsync();
            }
            return (record > 0) ? new Response { ResponseCode = (int)ResponseCodes.OK, SatisfactoryId = schedule_row_id } :
                                  new Response { ResponseCode = (int)ResponseCodes.Failed, ErrorCode = (int)ResponseErrorCodes.GeneralError };
        }
        #endregion

        #region APPOINTMENTS
        public async Task<Appointment?> FindAppoinmentAsync(int appointment_id) {
            return await this.context.Appointments.FirstOrDefaultAsync(x => x.AppointmentId == appointment_id);
        }

        public async Task<List<Appointment>> GetAppointmentsByUserAsync(int user_id) {
            return await this.context.Appointments.Where(x => x.UserId == user_id).ToListAsync();
        }

        public async Task<List<Appointment>> GetAppointmentsByHairdresserAsync(int hairdresser_id) {
            return await this.context.Appointments.Where(x => x.HairdresserId == hairdresser_id).ToListAsync();
        }

        public async Task<Response> InsertAppointmentAsync(int user_id, int hairdresser_id, DateTime date, TimeSpan time) {
            var newid = await this.context.Appointments.AnyAsync() ? await this.context.Appointments.MaxAsync(a => a.AppointmentId) + 1 : 1;
            Appointment appointment = new Appointment {
                AppointmentId = newid,
                UserId = user_id,
                HairdresserId = hairdresser_id,
                Date = date,
                Time = time,
                Status = (int)StatusAppointment.NoConfirmada
            };
            this.context.Appointments.Add(appointment);
            int record = await this.context.SaveChangesAsync();
            return (record > 0) ? new Response { ResponseCode = (int)ResponseCodes.OK, SatisfactoryId = newid } :
                                  new Response { ResponseCode = (int)ResponseCodes.Failed, ErrorCode = (int)ResponseErrorCodes.GeneralError };
        }

        public async Task<Response> UpdateAppointmentAsync(int appointment_id, DateTime date, TimeSpan time, StatusAppointment status) {
            Appointment? appointment = await this.FindAppoinmentAsync(appointment_id);
            int record = 0;
            if (appointment != null) {
                appointment.Date = date;
                appointment.Time = time;
                appointment.Status = status;
                record = await context.SaveChangesAsync();
            }
            return (record > 0) ? new Response { ResponseCode = (int)ResponseCodes.OK, SatisfactoryId = appointment_id } :
                                  new Response { ResponseCode = (int)ResponseCodes.Failed, ErrorCode = (int)ResponseErrorCodes.RecordNotFound };
        }

        public async Task<Response> DeleteAppointmentAsync(int appointment_id) {
            Appointment? appointment = await this.FindAppoinmentAsync(appointment_id);
            int record = 0;
            if (appointment != null) {
                List<int> app_services = await this.GetAppointmentServicesIDsAsync(appointment_id);
                foreach (int service in app_services) {
                    await this.DeleteAppointmentServiceAsync(appointment_id, service);
                }
                this.context.Appointments.Remove(appointment);
                record = await this.context.SaveChangesAsync();
            }
            return (record > 0) ? new Response { ResponseCode = (int)ResponseCodes.OK, SatisfactoryId = appointment_id } :
                                  new Response { ResponseCode = (int)ResponseCodes.Failed, ErrorCode = (int)ResponseErrorCodes.GeneralError };
        }

        public async Task<Response> ApproveAppointmentAsync(int appointment_id) {
            Appointment? appointment = await this.FindAppoinmentAsync(appointment_id);
            int record = 0;
            if (appointment != null) {
                appointment.Status = StatusAppointment.Activa;
                record = await this.context.SaveChangesAsync();
            }
            return (record > 0) ? new Response { ResponseCode = (int)ResponseCodes.OK, SatisfactoryId = appointment_id } :
                                  new Response { ResponseCode = (int)ResponseCodes.Failed, ErrorCode = (int)ResponseErrorCodes.GeneralError };
        }
        #endregion

        #region SERVICES
        public async Task<Service?> FindServiceAsync(int service_id) {
            return await this.context.Services.FirstOrDefaultAsync(s => s.ServiceId == service_id);
        }

        public async Task<List<Service>> GetServicesByAppointmentAsync(int appointment_id) {
            List<int> app_services = await this.GetAppointmentServicesIDsAsync(appointment_id);
            List<Service> services = new List<Service>();
            foreach (int app_service_id in app_services) {
                Service? newService = await FindServiceAsync(app_service_id);
                if (newService != null) { services.Add(newService); }
            }
            return services;
        }

        public async Task<List<Service>> GetServicesByHairdresserAsync(int hairdresser_id) {
            return await context.Services.Where(s => s.HairdresserId == hairdresser_id).ToListAsync();
        }

        public async Task<List<Service>> GetServicesByIdentificationAsync(List<int> app_services) {
            List<Service> services = new List<Service>();
            foreach (int app_service_id in app_services) {
                Service? newService = await this.FindServiceAsync(app_service_id);
                if (newService != null) { services.Add(newService); }
            }
            return services;
        }

        public async Task<Response> InsertServiceAsync(int hairdresser_id, string name, decimal price, byte duration) {
            var newid = await this.context.Services.AnyAsync() ? await this.context.Services.MaxAsync(a => a.ServiceId) + 1 : 1;
            Service service = new Service {
                ServiceId = newid,
                HairdresserId = hairdresser_id,
                Name = name,
                Price = price,
                TiempoAprox = duration
            };
            this.context.Services.Add(service);
            int record = await this.context.SaveChangesAsync();
            return (record > 0) ? new Response { ResponseCode = (int)ResponseCodes.OK, SatisfactoryId = newid } :
                                  new Response { ResponseCode = (int)ResponseCodes.Failed, ErrorCode = (int)ResponseErrorCodes.GeneralError };
        }

        public async Task<Response> UpdateServiceAsync(int service_id, string name, decimal price, byte duration) {
            Service? service = await this.FindServiceAsync(service_id);
            int record = 0;
            if (service != null) {
                service.Name = name;
                service.Price = price;
                service.TiempoAprox = duration;
                record = await this.context.SaveChangesAsync();
            }
            return (record > 0) ? new Response { ResponseCode = (int)ResponseCodes.OK, SatisfactoryId = service_id } :
                                  new Response { ResponseCode = (int)ResponseCodes.Failed, ErrorCode = (int)ResponseErrorCodes.RecordNotFound };
        }

        public async Task<Response> DeleteServiceAsync(int service_id) {
            Service? service = await this.FindServiceAsync(service_id);
            int record = 0;
            if (service != null) {
                this.context.Services.Remove(service);
                record = await this.context.SaveChangesAsync();
            }
            return (record > 0) ? new Response { ResponseCode = (int)ResponseCodes.OK, SatisfactoryId = service_id } :
                      new Response { ResponseCode = (int)ResponseCodes.Failed, ErrorCode = (int)ResponseErrorCodes.RecordNotFound };
        }
        #endregion

        #region APPOINTMENT_SERVICES
        public async Task<Appointment_Service?> FindAppointmentServiceAsync(int appointment_id, int service_id) {
            return await this.context.AppointmentServices.FirstOrDefaultAsync(a => a.AppointmentId == appointment_id && a.ServiceId == service_id);
        }

        public async Task<List<int>> GetAppointmentServicesIDsAsync(int appointment_id) {
            var query = from data in context.AppointmentServices
                        where data.AppointmentId == appointment_id
                        select data.ServiceId;
            return await query.ToListAsync();
        }

        public async Task<List<Appointment_Service>> GetAppointmentServicesAsync(int appointment_id) {
            return await this.context.AppointmentServices.Where(a => a.AppointmentId == appointment_id).ToListAsync();
        }

        public async Task<Response> InsertAppointmentServiceAsync(int appointment_id, int service_id) {
            Appointment_Service? appointmentService = await FindAppointmentServiceAsync(appointment_id, service_id);
            int record = 0;
            if (appointmentService == null) {
                Appointment_Service newAppointmentService = new Appointment_Service {
                    AppointmentId = appointment_id,
                    ServiceId = service_id
                };
                this.context.AppointmentServices.Add(newAppointmentService);
                record = await this.context.SaveChangesAsync();
            }
            return (record > 0) ? new Response { ResponseCode = (int)ResponseCodes.OK, SatisfactoryId = appointment_id } :
                                  new Response { ResponseCode = (int)ResponseCodes.Failed, ErrorCode = (int)ResponseErrorCodes.GeneralError };
        }

        public async Task<Response> DeleteAppointmentServiceAsync(int appointment_id, int service_id) {
            Appointment_Service? appointmentService = await this.FindAppointmentServiceAsync(appointment_id, service_id);
            int record = 0;
            if (appointmentService != null) {
                this.context.AppointmentServices.Remove(appointmentService);
                record = await this.context.SaveChangesAsync();
            }
            return (record > 0) ? new Response { ResponseCode = (int)ResponseCodes.OK, SatisfactoryId = appointment_id } :
                                  new Response { ResponseCode = (int)ResponseCodes.Failed, ErrorCode = (int)ResponseErrorCodes.GeneralError };
        }
        #endregion
        */
    }
}