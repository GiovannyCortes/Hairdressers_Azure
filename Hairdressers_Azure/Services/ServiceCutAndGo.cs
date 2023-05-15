using Azure.Security.KeyVault.Secrets;
using CutAndGo.Models;
using Newtonsoft.Json;
using NugetHairdressersAzure.Models;
using System.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Text;


namespace Hairdressers_Azure.Services {
    public class ServiceCutAndGo {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private MediaTypeWithQualityHeaderValue _header;
        private string _urlApi;

        public ServiceCutAndGo(IHttpContextAccessor httpContextAccessor, SecretClient secretClient) {
            this._httpContextAccessor = httpContextAccessor;
            this._header = new MediaTypeWithQualityHeaderValue("application/json");
            this._urlApi = GetUrlApiAsync(secretClient).Result;
        }

        private static async Task<string> GetUrlApiAsync(SecretClient secretClient) {
            KeyVaultSecret keyVaultSecret = await secretClient.GetSecretAsync("apicutandgo");
            return keyVaultSecret.Value;
        }

        #region GENERAL TOKEN
        private async Task<T> CallApiAsync<T>(string request, string token) {
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(this._urlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this._header);

                if (token != null) {
                    client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                }

                HttpResponseMessage response = await client.GetAsync(request);
                return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<T>() : default(T);
            }
        }

        private async Task<string?> InsertApiAsync<T>(string request, T objeto, string token) {
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(this._urlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this._header);
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

        private async Task<string?> UpdateApiAsync<T>(string request, T objeto, string token) {
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(this._urlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this._header);

                if (token != null) {
                    client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
                }

                string json = JsonConvert.SerializeObject(objeto);

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(request, content);
                if (response.IsSuccessStatusCode) {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return responseContent;
                } else {
                    return null;
                }
            }
        }

        // Se supone que en el request ya va el id. Ejemplo: http:/localhost/api/deletealgo/17
        private async Task<HttpStatusCode> DeleteApiAsync(string request, string token) {
            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(this._urlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + token);

                HttpResponseMessage response = await client.DeleteAsync(request);
                return response.StatusCode;
            }
        }
        #endregion

        #region LOGIN
        public async Task<string> LogInAsync(string email, string password) {
            LogIn login = new LogIn { 
                UserMail = email,
                Password = password
            };

            string request = "/api/Auth/LogIn";
            string json = JsonConvert.SerializeObject(login);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient()) {
                client.BaseAddress = new Uri(this._urlApi);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this._header);

                HttpResponseMessage response = await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode) {
                    string responseToken = await response.Content.ReadAsStringAsync();
                    return responseToken;
                } else {
                    return default(string);
                }
            }
        }
        #endregion

        #region TOKENS
        public async Task<string> GenerateToken() {
            string request = "/api/tokens/GenerateToken";
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<string>(request, contextToken);
        }

        public async Task<bool> UserAssignTokenAsync(int user_id, string token) {
            string request = "/api/tokens/UserAssignToken/" + user_id + "/" + token;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            string responseContent = await this.InsertApiAsync<string>(request, null, contextToken);
            return JsonConvert.DeserializeObject<bool>(responseContent);
        }

        public async Task<bool> UserValidateTokenAsync(int user_id, string token) {
            string request = "/api/tokens/UserValidateToken/" + user_id + "/" + token;
            return await this.CallApiAsync<bool>(request, null);
        }

        public async Task<bool> HairdresserValidateTokenAsync(int hairdresser_id, string token) {
            string request = "/api/tokens/HairdresserValidateToken/" + hairdresser_id + "/" + token;
            return await this.CallApiAsync<bool>(request, null);
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

            await this.UpdateApiAsync<UserUpdates>(request, user, contextToken);
        }

        public async Task DeleteUserAsync(int user_id) {
            string request = "/api/users/delete/" + user_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.DeleteApiAsync(request, contextToken);
        }

        public async Task<int> ValidateEmailAsync(int user_id, string token) {
            string request = "/api/users/ValidateEmail/" + user_id + "/" + token;
            string responseContent = await this.UpdateApiAsync<User>(request, null, null);
            return JsonConvert.DeserializeObject<int>(responseContent);
        }
        #endregion
        
        #region HAIRDRESSERS
        public async Task<Hairdresser?> FindHairdresserAsync(int hairdresser_id) {
            string request = "/api/Hairdressers/FindHairdresser/" + hairdresser_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<Hairdresser?>(request, contextToken);
        }

        public async Task<List<string>> GetHairdresserEmailsAsync(int hairdresser_id) {
            string request = "/api/Hairdressers/GetHairdresserEmails/" + hairdresser_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<List<string>>(request, contextToken);
        }

        public async Task<List<Hairdresser>> GetHairdressersAsync() {
            string request = "/api/Hairdressers/GetHairdressers";
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<List<Hairdresser>>(request, contextToken);
        }

        public async Task<List<Hairdresser>> GetHairdressersByUserAsync(int user_id) {
            string request = "/api/Hairdressers/GetHairdressersByUser/" + user_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<List<Hairdresser>>(request, contextToken);
        }

        public async Task<List<Hairdresser>> GetHairdressersByFilterNameAsync(string filterName) {
            string request = "/api/Hairdressers/GetHairdressersByFilterName/" + filterName;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<List<Hairdresser>>(request, contextToken);
        }

        public async Task<int> InsertHairdresserAsync(string name, string phone, string address, int postal_code, string image_extension, int user_id) {
            string request = "/api/Hairdressers/Create";
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");

            HairdresserRegister hairdresserRegister = new HairdresserRegister { 
                Name = name,
                Phone = phone,
                Address = address,
                PostalCode = postal_code,
                ImageExtension = image_extension,
                UserId = user_id
            };

            string responseContent = await this.InsertApiAsync<HairdresserRegister>(request, hairdresserRegister, contextToken);
            return JsonConvert.DeserializeObject<int>(responseContent);
        }

        public async Task UpdateHairdresserAsync(int hairdresser_id, string name, string phone, string address, int postal_code, string image_extension) {
            string request = "/api/Hairdressers/Update";
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");

            HairdresserUpdates hairdresserUpdate= new HairdresserUpdates {
                HairdresserId = hairdresser_id,
                Name = name,
                Phone = phone,
                Address = address,
                PostalCode = postal_code,
                ImageExtension = image_extension,
            };

            await this.UpdateApiAsync<HairdresserUpdates>(request, hairdresserUpdate, contextToken);
        }

        public async Task DeleteHairdresserAsync(int hairdresser_id) {
            string request = "/api/Hairdressers/Delete/" + hairdresser_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.DeleteApiAsync(request, contextToken);
        }
        #endregion

        #region SCHEDULES
        public async Task<Schedule?> FindScheduleAsync(int schedule_id, bool getRows) {
            string request = "/api/Schedules/FindSchedule/" + schedule_id + "/" + getRows;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<Schedule?>(request, contextToken);
        }

        public async Task<Schedule?> FindActiveScheduleAsync(int hairdresser_id, bool getRows) {
            string request = "/api/Schedules/FindActiveSchedule/" + hairdresser_id + "/" + getRows;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<Schedule?>(request, contextToken);
        }

        public async Task<List<string>> GetNameSchedulesAsync(int hairdresser_id) {
            string request = "/api/Schedules/GetNameSchedules/" + hairdresser_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<List<string>>(request, contextToken);
        }

        public async Task<List<Schedule>> GetSchedulesAsync(int hairdresser_id, bool getRows) {
            string request = "/api/Schedules/GetSchedules/" + hairdresser_id + "/" + getRows;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<List<Schedule>>(request, contextToken);
        }

        public async Task<int> InsertScheduleAsync(int hairdresser_id, string name, bool active) {
            string request = "/api/Schedules/Create";
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");

            ScheduleRegister scheduleRegister = new ScheduleRegister {
                Name = name,
                Active = active,
                HairdresserId = hairdresser_id
            };

            string responseContent = await this.InsertApiAsync<ScheduleRegister>(request, scheduleRegister, contextToken);
            return JsonConvert.DeserializeObject<int>(responseContent);
        }

        public async Task UpdateScheduleAsync(int schedule_id, int hairdresser_id, string name, bool active) {
            string request = "/api/Schedules/Update";
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");

            ScheduleUpdates scheduleUpdate = new ScheduleUpdates {
                ScheduleId = schedule_id,
                Name = name,
                Active = active,
                HairdresserId = hairdresser_id
            };

            await this.UpdateApiAsync<ScheduleUpdates>(request, scheduleUpdate, contextToken);
        }

        public async Task<int> DeleteScheduleAsync(int schedule_id) {
            string request = "/api/Schedules/Delete/" + schedule_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.DeleteApiAsync(request, contextToken);
            Schedule? schedule = await this.FindScheduleAsync(schedule_id, false);
            return (schedule != null) ? schedule.ScheduleId : 0;
        }

        public async Task ActivateScheduleAsync(int hairdresser_id, int schedule_id) {
            string request = "/api/Schedules/ActivateSchedule/" + hairdresser_id + "/" + schedule_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.UpdateApiAsync<ScheduleUpdates>(request, null, contextToken);
        }
        #endregion

        #region SCHEDULE_ROWS
        public async Task<Schedule_Row?> FindScheduleRowAsync(int schedule_row_id) {
            string request = "/api/ScheduleRows/FindScheduleRow/" + schedule_row_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<Schedule_Row?>(request, contextToken);
        }

        public async Task<List<Schedule_Row>> GetScheduleRowsAsync(int schedule_id) {
            string request = "/api/ScheduleRows/GetScheduleRows/" + schedule_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<List<Schedule_Row>>(request, contextToken);
        }

        public async Task<List<Schedule_Row>> GetActiveScheduleRowsAsync(int hairdresser_id) {
            string request = "/api/ScheduleRows/GetActiveScheduleRows/" + hairdresser_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<List<Schedule_Row>>(request, contextToken);
        }

        public async Task<int> InsertScheduleRowsAsync(int schid, TimeSpan start, TimeSpan end, bool mon, bool tue, bool wed, bool thu, bool fri, bool sat, bool sun) {
            string request = "/api/ScheduleRows/Create";
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");

            Schedule_RowRegister schedule_RowRegister = new Schedule_RowRegister {
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

            string responseContent = await this.InsertApiAsync<Schedule_RowRegister>(request, schedule_RowRegister, contextToken);
            return (responseContent != null) ? JsonConvert.DeserializeObject<int>(responseContent) : -1;
        }

        public async Task DeleteScheduleRowsAsync(int schedule_row_id) {
            string request = "/api/ScheduleRows/Delete/" + schedule_row_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.DeleteApiAsync(request, contextToken);
        }
        #endregion

        #region APPOINTMENTS
        public async Task<Appointment?> FindAppoinmentAsync(int appointment_id) {
            string request = "/api/Appointments/FindAppointment/" + appointment_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<Appointment?>(request, contextToken);
        }

        public async Task<List<Appointment>> GetAppointmentsByUserAsync(int user_id) {
            string request = "/api/Appointments/GetAppointmentsByUser/" + user_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<List<Appointment>>(request, contextToken);
        }

        public async Task<List<Appointment>> GetAppointmentsByHairdresserAsync(int hairdresser_id) {
            string request = "/api/Appointments/GetAppointmentsByHairdresser/" + hairdresser_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<List<Appointment>>(request, contextToken);
        }

        public async Task<int> InsertAppointmentAsync(int user_id, int hairdresser_id, DateTime date, TimeSpan time) {
            string request = "/api/Appointments/Create";
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");

            AppointmentRegister appointmentRegister = new AppointmentRegister { 
                UserId = user_id,
                HairdresserId = hairdresser_id,
                Date = date,
                Time = time
            };

            string responseContent = await this.InsertApiAsync<AppointmentRegister>(request, appointmentRegister, contextToken);
            return JsonConvert.DeserializeObject<int>(responseContent);
        }

        public async Task UpdateAppointmentAsync(int appointment_id, DateTime date, TimeSpan time, StatusAppointment status) {
            string request = "/api/Appointments/Update";
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");

            AppointmentUpdates appointmentUpdate = new AppointmentUpdates {
                AppointmentId = appointment_id,
                Date = date,
                Time = time,
                Status = status
            };

            await this.UpdateApiAsync<AppointmentUpdates>(request, appointmentUpdate, contextToken);
        }

        public async Task DeleteAppointmentAsync(int appointment_id) {
            string request = "/api/Appointments/Delete/" + appointment_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.DeleteApiAsync(request, contextToken);
        }

        public async Task ApproveAppointmentAsync(int appointment_id) {
            string request = "/api/Appointments/ApproveAppointment/" + appointment_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.UpdateApiAsync<AppointmentUpdates>(request, null, contextToken);
        }
        #endregion

        #region SERVICES
        public async Task<Service?> FindServiceAsync(int service_id) {
            string request = "/api/Services/FindService/" + service_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<Service?>(request, contextToken);
        }

        public async Task<List<Service>> GetServicesByAppointmentAsync(int appointment_id) {
            string request = "/api/Services/GetServicesByAppointment/" + appointment_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<List<Service>>(request, contextToken);
        }

        public async Task<List<Service>> GetServicesByHairdresserAsync(int hairdresser_id) {
            string request = "/api/Services/GetServicesByHairdresser/" + hairdresser_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<List<Service>>(request, contextToken);
        }

        public async Task<List<Service>> GetServicesByIdentificationAsync(List<int> app_services) {
            string request = "/api/Services/GetServicesByIdentification";
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");

            string json = JsonConvert.SerializeObject(app_services);
            string base64Json = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

            // agregar el cuerpo codificado en base64 como parte de la URL
            string url = $"{this._urlApi}{request}?json={base64Json}";

            using (HttpClient client = new HttpClient()) {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this._header);
                client.DefaultRequestHeaders.Add("Authorization", "bearer " + contextToken);

                // crear la solicitud GET con el cuerpo codificado en base64 en la URL
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

                HttpResponseMessage response = await client.SendAsync(requestMessage);
                return response.IsSuccessStatusCode ? await response.Content.ReadAsAsync<List<Service>>() : default(List<Service>);
            }
        }


        public async Task<int> InsertServiceAsync(int hairdresser_id, string name, decimal price, byte duration) {
            string request = "/api/Services/Create";
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");

            ServiceRegister serviceRegister = new ServiceRegister {
                HairdresserId = hairdresser_id,
                Name = name,
                Price = price,
                TiempoAprox = duration
            };

            string responseContent = await this.InsertApiAsync<ServiceRegister>(request, serviceRegister, contextToken);
            return JsonConvert.DeserializeObject<int>(responseContent);
        }

        public async Task UpdateServiceAsync(int service_id, string name, decimal price, byte duration) {
            string request = "/api/Services/Update";
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");

            ServiceUpdates serviceUpdate = new ServiceUpdates {
                ServiceId = service_id,
                Name = name,
                Price = price,
                TiempoAprox = duration
            };

            await this.UpdateApiAsync<ServiceUpdates>(request, serviceUpdate, contextToken);
        }

        public async Task DeleteServiceAsync(int service_id) {
            string request = "/api/Services/Delete/" + service_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.DeleteApiAsync(request, contextToken);
        }
        #endregion

        #region APPOINTMENT_SERVICES
        public async Task<Appointment_Service?> FindAppointmentServiceAsync(int appointment_id, int service_id) {
            string request = "/api/AppointmentServices/FindAppointmentService/" + appointment_id + "/" + service_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<Appointment_Service?>(request, contextToken);
        }

        public async Task<List<int>> GetAppointmentServicesIDsAsync(int appointment_id) {
            string request = "/api/AppointmentServices/GetAppointmentServicesIDs/" + appointment_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<List<int>>(request, contextToken);
        }

        public async Task<List<Appointment_Service>> GetAppointmentServicesAsync(int appointment_id) {
            string request = "/api/AppointmentServices/GetAppointmentServices/" + appointment_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            return await this.CallApiAsync<List<Appointment_Service>>(request, contextToken);
        }

        public async Task InsertAppointmentServiceAsync(int appointment_id, int service_id) {
            string request = "/api/AppointmentServices/Create/" + appointment_id + "/" + service_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.InsertApiAsync<Appointment_Service>(request, null, contextToken);
        }

        public async Task DeleteAppointmentServiceAsync(int appointment_id, int service_id) {
            string request = "/api/AppointmentServices/Delete/" + appointment_id + "/" + service_id;
            string contextToken = _httpContextAccessor.HttpContext.Session.GetString("TOKEN");
            await this.DeleteApiAsync(request, contextToken);
        }
        #endregion
        
    }
}