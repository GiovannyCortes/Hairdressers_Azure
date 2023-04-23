using Hairdressers_Azure.Data;
using Hairdressers_Azure.Helpers;
using Hairdressers_Azure.Interfaces;
using Hairdressers_Azure.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

#region NO
/*

    CREATE PROCEDURE SP_COMPARE_ROLE(@HAIRDRESSER_ID INT, @USER_ID1 INT, @USER_ID2 INT, @RES BIT OUT)
    AS
	    DECLARE @ROLE_1 INT, @ROLE_2 INT;
	    SELECT @ROLE_1 = ADMINS.role FROM ADMINS WHERE ADMINS.[user_id] = @USER_ID1 AND ADMINS.hairdresser_id = @HAIRDRESSER_ID;
	    SELECT @ROLE_2 = ADMINS.role FROM ADMINS WHERE ADMINS.[user_id] = @USER_ID2 AND ADMINS.hairdresser_id = @HAIRDRESSER_ID;
	    IF(@ROLE_1 <= @ROLE_2)
		    SET @RES = 1;
	    ELSE
		    SET @RES = 0;
    GO



 */
#endregion

/*    CREATE PROCEDURE SP_ASSIGN_TOKEN(@USER_ID INT, @TOKEN NVARCHAR(100))
    AS
	    UPDATE USERS SET TEMP_TOKEN = @TOKEN WHERE USER_ID = @USER_ID;
    GO
     CREATE PROCEDURE SP_GET_HAIRDRESSER_EMAILS (@HAIRDRESSER_ID INT)
    AS
	    SELECT EMAIL 
	    FROM USERS INNER JOIN ADMINS ON ADMINS.user_id = USERS.user_id
	    WHERE ADMINS.hairdresser_id = 1
    GO
 */


namespace Hairdressers_Azure.Repositories {

    public enum ServerRes { OK = 0, ExistingRecord = 1, RecordNotFound = 2, DeleteWithOneAdmin = 3, NotAuthorized = 4 }
    public enum Validates { No_Encontrado = -1, Ok = 0, Rango_Sobreescrito = 1, Duplicado = 2, Rango_incorrecto = 3 }

    public class RepositoryHairdresser {

        HairdressersContext context;

        public RepositoryHairdresser(HairdressersContext context) {
            this.context = context;
        }

        #region TOKENS
        public string GenerateToken() {
            const string caracteresPermitidos = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var token = new string(
                    Enumerable.Repeat(caracteresPermitidos, 50).Select(s => s[random.Next(s.Length)]).ToArray()
                );
            return token;
        }

        public async Task<bool> ValidateTokenAsync(int user_id, string token) {
            var consulta = from data in context.Users
                           where data.UserId == user_id
                           select new User {
                               TempToken = data.TempToken ?? ""
                           };
            User? user = await consulta.FirstOrDefaultAsync();
            if (user != null && user.TempToken != null) {
                return user.TempToken == token;
            } else {
                return false;
            }
        }
        #endregion

        #region USER
        public async Task<User?> FindUserAsync(int user_id) {
            var user = await context.Users
                           .Where(u => u.UserId == user_id)
                           .FirstOrDefaultAsync();

            if (user != null) {
                user.LastName = user.LastName ?? "";
                user.Phone = user.Phone ?? "Sin número de teléfono";
                user.TempToken = user.TempToken ?? "";
            }

            return user;
        }

        public async Task<User?> ValidateUserAsync(string email, string password) {
            var consulta = from data in context.Users
                           where data.Email == email
                           select new User {
                               UserId = data.UserId,
                               Salt = data.Salt,
                               Password = data.Password,
                               PasswordRead = data.PasswordRead,
                               Name = data.Name,
                               LastName = data.LastName,
                               Phone = data.Phone ?? "Sin número de teléfono",
                               Email = data.Email,
                               EmailConfirmed = data.EmailConfirmed,
                               TempToken = data.TempToken ?? ""
                           };
            User? user = await consulta.FirstOrDefaultAsync();
            if (user == null) {
                return null;
            } else {
                byte[] passUsuario = user.Password;
                byte[] temp = HelperCryptography.EncryptContent(password, user.Salt);

                bool respuesta = HelperCryptography.CompareArrays(passUsuario, temp);
                return respuesta ? user : null;
            }
        }

        public async Task<User> InsertUserAsync(string password, string name, string lastname, string phone, string email, bool econfirmed) {
            var newid = context.Users.Any() ? context.Users.Max(u => u.UserId) + 1 : 1;
            string salt = HelperCryptography.GenerateSalt();

            User user = new User {
                UserId = newid,
                Salt = salt,
                Password = HelperCryptography.EncryptContent(password, salt),
                PasswordRead = password,
                Name = name,
                LastName = lastname,
                Phone = phone,
                Email = email,
                EmailConfirmed = econfirmed,
                TempToken = ""
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateUserAsync(int user_id, string name, string lastname, string phone, string email) {
            User? user = await FindUserAsync(user_id);
            if (user != null) {
                user.Name = name;
                user.LastName = lastname;
                user.Phone = phone;
                user.Email = email;
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsAdmin(int user_id) {
            return await context.Admins.AnyAsync(a => a.UserId == user_id);
        }

        public async Task<bool> EmailExist(string email) {
            return await context.Users.AnyAsync(a => a.Email == email);
        }

        public async Task ValidateEmailAsync(int user_id) {
            User? user = await FindUserAsync(user_id);
            if (user != null) {
                user.EmailConfirmed = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task AssignTokenAsync(int user_id, string token) {
            string procedure = "SP_ASSIGN_TOKEN @USER_ID, @TOKEN";
            SqlParameter pamUserId = new SqlParameter("@USER_ID", user_id);
            SqlParameter pamToken = new SqlParameter("@TOKEN", token);
            await context.Database.ExecuteSqlRawAsync(procedure, pamUserId, pamToken);
        }
        #endregion

        #region ADMIN
        public async Task<bool> AdminExistAsync(int hairdresser_id, int user_id) {
            return await context.Admins.AnyAsync(admin => admin.UserId == user_id && admin.HairdresserId == hairdresser_id);
        }

        public async Task<Admin?> FindAdminAsync(int hairdresser_id, int user_id) {
            var consulta = from datos in context.Admins
                           where datos.HairdresserId == hairdresser_id && datos.UserId == user_id
                           select datos;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<List<Admin>> GetAdminsAsync(int hairdresser_id) {
            var consulta = from datos in context.Admins
                           where datos.HairdresserId == hairdresser_id
                           select datos;
            return await consulta.ToListAsync();
        }

        public bool CompareAdminRole(int hairdresser_id, int user_id_action, int user_id_affect) {
            string sql = "SP_COMPARE_ROLE @HAIRDRESSER_ID, @USER_ID1, @USER_ID2, @RES OUT";

            SqlParameter pam_hid = new SqlParameter("@HAIRDRESSER_ID", hairdresser_id);
            SqlParameter pam_us1 = new SqlParameter("@USER_ID1", user_id_action);
            SqlParameter pam_us2 = new SqlParameter("@USER_ID2", user_id_affect);
            SqlParameter pam_res = new SqlParameter("@RES", SqlDbType.Bit);
            pam_res.Direction = ParameterDirection.Output;

            context.Database.ExecuteSqlRaw(sql, pam_hid, pam_us1, pam_us2, pam_res);
            return (bool)pam_res.Value;
        }

        public async Task<int> InsertAdminAsync(int hairdresser_id, int user_id, AdminRole role) {
            Admin? admin = await FindAdminAsync(hairdresser_id, user_id);
            if (admin == null) {
                Admin new_admin = new Admin {
                    HairdresserId = hairdresser_id,
                    UserId = user_id,
                    Role = role
                };
                context.Admins.Add(new_admin);
                await context.SaveChangesAsync();
                return (int)ServerRes.OK;
            } else { return (int)ServerRes.ExistingRecord; }
        }

        public async Task<int> UpdateAdminAsync(int hairdresser_id, int user_id, AdminRole role) {
            Admin? admin = await FindAdminAsync(hairdresser_id, user_id);
            if (admin != null) {
                admin.Role = role;
                await context.SaveChangesAsync();
                return (int)ServerRes.OK;
            } else { return (int)ServerRes.RecordNotFound; }
        }

        public async Task<int> DeleteAdminAsync(int hairdresser_id, int user_id_affect, int user_id_action) {
            if (CompareAdminRole(hairdresser_id, user_id_action, user_id_affect)) { // Devolverá TRUE si el rango es >= que el afectado
                if ((await GetAdminsAsync(hairdresser_id)).Count == 1) {
                    return (int)ServerRes.DeleteWithOneAdmin;
                } else {
                    Admin? admin = await FindAdminAsync(hairdresser_id, user_id_affect);
                    if (admin != null) {
                        context.Admins.Remove(admin);
                        await context.SaveChangesAsync();
                    }
                    return (int)ServerRes.OK;
                }
            } else { return (int)ServerRes.NotAuthorized; }
        }
        #endregion

        #region HAIRDRESSER
        public async Task<Hairdresser?> FindHairdresserAsync(int hairdresser_id) {
            var hairdresser = await context.Hairdressers
                                    .Where(h => h.HairdresserId == hairdresser_id)
                                    .FirstOrDefaultAsync();
            return hairdresser;
        }

        public async Task<List<Hairdresser>> GetHairdressersAsync() {
            var consulta = from data in context.Hairdressers
                           select new Hairdresser {
                               HairdresserId = data.HairdresserId,
                               Name = data.Name,
                               Address = data.Address,
                               PostalCode = data.PostalCode,
                               Phone = data.Phone ?? "Sin número de teléfono",
                               Token = data.Token
                           };
            return await consulta.ToListAsync();
        }

        public async Task<List<Hairdresser>> GetHairdressersAsync(int user_id) {
            var consulta = context.Hairdressers
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
                           .Where(x => x.User.UserId == user_id)
                           .Select(x => new Hairdresser {
                               HairdresserId = x.Hairdresser.HairdresserId,
                               Name = x.Hairdresser.Name,
                               Address = x.Hairdresser.Address,
                               PostalCode = x.Hairdresser.PostalCode,
                               Phone = x.Hairdresser.Phone ?? "Sin número de teléfono",
                               Token = x.Hairdresser.Token
                           });
            return await consulta.ToListAsync();
        }

        public async Task<List<Hairdresser>> GetHairdressersByFilter(string filter) {
            var consulta = from data in context.Hairdressers
                           where data.Name.ToLower().Contains(filter.ToLower())
                           select new Hairdresser {
                               HairdresserId = data.HairdresserId,
                               Name = data.Name,
                               Address = data.Address,
                               PostalCode = data.PostalCode,
                               Phone = data.Phone ?? "",
                               Token = data.Token
                           };
            return await consulta.ToListAsync();

        }

        public async Task<int> InsertHairdresserAsync(string name, string phone, string address, int postal_code, int user_id) {
            var newid = context.Hairdressers.Any() ? context.Hairdressers.Max(s => s.HairdresserId) + 1 : 1;
            Hairdresser hairdresser = new Hairdresser {
                HairdresserId = newid,
                Name = name,
                Phone = phone,
                Address = address,
                PostalCode = postal_code,
                Token = GenerateToken()
            };
            context.Hairdressers.Add(hairdresser);

            await InsertAdminAsync(newid, user_id, AdminRole.Propietario);

            await context.SaveChangesAsync();
            return newid;
        }

        public async Task UpdateHairdresserAsync(int hairdresser_id, string name, string phone, string address, int postal_code) {
            Hairdresser? hairdresser = await FindHairdresserAsync(hairdresser_id);
            if (hairdresser != null) {
                hairdresser.Name = name;
                hairdresser.Phone = phone;
                hairdresser.Address = address;
                hairdresser.PostalCode = postal_code;
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteHairdresserAsync(int hairdresser_id) {
            Hairdresser? hairdresser = await FindHairdresserAsync(hairdresser_id);
            if (hairdresser != null) {
                /// 1 - Borrar los registros de Admin
                List<Admin> admins = await GetAdminsAsync(hairdresser_id);
                foreach (Admin admin in admins) {
                    context.Admins.Remove(admin);
                }
                await context.SaveChangesAsync();

                /// 2 - Borrar los registros de Horario
                List<Schedule> schedules = await GetSchedulesAsync(hairdresser_id, false);
                foreach (Schedule schedule in schedules) {
                    List<Schedule_Row> schedule_Rows = await GetScheduleRowsAsync(schedule.ScheduleId);
                    foreach (Schedule_Row schedule_Row in schedule_Rows) {
                        context.Schedule_Rows.Remove(schedule_Row);
                    }
                    await context.SaveChangesAsync();
                    context.Schedules.Remove(schedule);
                }
                await context.SaveChangesAsync();

                /// 3 - Borrar los registros de citas y sus relaciones con servicios
                List<Appointment> appointments = await GetAppointmentsByHairdresserAsync(hairdresser_id);
                foreach (Appointment appointment in appointments) {
                    List<Appointment_Service> appointment_Services = await GetObjectAppointmentServiceAsync(appointment.AppointmentId);
                    foreach (Appointment_Service appointment_Service in appointment_Services) {
                        context.AppointmentServices.Remove(appointment_Service);
                    }
                    await context.SaveChangesAsync();
                    context.Appointments.Remove(appointment);
                }
                await context.SaveChangesAsync();

                /// 4 - Borrar los registros de servicios
                List<Service> services = await GetServicesByHairdresserAsync(hairdresser_id);
                foreach (Service service in services) {
                    context.Services.Remove(service);
                }
                await context.SaveChangesAsync();

                /// 5 - Eliminar la peluquería
                context.Hairdressers.Remove(hairdresser);
                await context.SaveChangesAsync();
            }
        }

        public async Task<string> GetHairdresserEmailsAsync(int hairdresser_id) {
            var correos = await (from usuario in context.Users
                                 join admin in context.Admins
                                 on usuario.UserId equals admin.UserId
                                 where admin.HairdresserId == hairdresser_id
                                 select usuario.Email).ToListAsync();

            return string.Join(",", correos);
        }

        public async Task<bool> CompareHairdresserTokenAsync(int hairdresser_id, string token) {
            Hairdresser? hairdresser = await FindHairdresserAsync(hairdresser_id);
            if (hairdresser != null) {
                return hairdresser.Token.Equals(token);
            } else {
                return false;
            }
        }
        #endregion

        #region SCHEDULE
        public async Task<List<string>> GetNameSchedulesAsync(int hairdresser_id) {
            var consulta = from data in context.Schedules
                           where data.HairdresserId == hairdresser_id
                           select data.Name;
            return await consulta.ToListAsync();
        }

        public async Task<List<Schedule>> GetSchedulesAsync(int hairdresser_id, bool getrows) {
            var consulta = from data in context.Schedules
                           where data.HairdresserId == hairdresser_id
                           select data;
            List<Schedule> schedules = await consulta.ToListAsync();
            if (getrows) {
                foreach (Schedule sch in schedules) {
                    List<Schedule_Row> schedule_rows = await GetScheduleRowsAsync(sch.ScheduleId);
                    foreach (Schedule_Row row in schedule_rows) {
                        sch.ScheduleRows.Add(row);
                    }
                }
            }
            return schedules;
        }

        public async Task<Schedule?> FindScheduleAsync(int schedule_id, bool getrows) {
            var consulta = from data in context.Schedules
                           where data.ScheduleId == schedule_id
                           select data;
            Schedule? schedule = await consulta.FirstOrDefaultAsync();
            if (schedule != null && getrows) {
                List<Schedule_Row> schedule_rows = await GetScheduleRowsAsync(schedule.ScheduleId);
                foreach (Schedule_Row row in schedule_rows) {
                    schedule.ScheduleRows.Add(row);
                }
            }
            return schedule;
        }

        public async Task<Schedule?> FindActiveScheduleAsync(int hairdresser_id) {
            var consulta = from data in context.Schedules
                           where data.HairdresserId == hairdresser_id && data.Active == true
                           select data;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<int> InsertScheduleAsync(int hairdresser_id, string name, bool active) {
            List<Schedule> schedules = await GetSchedulesAsync(hairdresser_id, false);
            bool duplicado = false;
            foreach (Schedule sch in schedules) {
                if (sch.Name.ToLower() == name.ToLower()) { duplicado = true; }
            }

            if (!duplicado) { // No hay duplicados
                if (active) {
                    foreach (Schedule sch in schedules) {
                        if (sch.Active) { sch.Active = false; }
                    }
                }

                var newid = context.Schedules.Any() ? context.Schedules.Max(s => s.ScheduleId) + 1 : 1;
                Schedule schedule = new Schedule {
                    ScheduleId = newid,
                    HairdresserId = hairdresser_id,
                    Name = name,
                    Active = active
                };

                context.Schedules.Add(schedule);
                await context.SaveChangesAsync();
                return newid;
            } else {
                return 0;
            }
        }

        public async Task UpdateScheduleAsync(int schedule_id, int hairdresser_id, string name, bool active) {
            Schedule? schedule = await FindScheduleAsync(schedule_id, false);
            if (schedule != null) {
                schedule.Name = name;
                schedule.Active = active;

                if (active) {
                    List<Schedule> schedules = await GetSchedulesAsync(hairdresser_id, false);
                    foreach (Schedule sch in schedules) {
                        if (sch.ScheduleId != schedule_id && sch.Active) { sch.Active = false; }
                    }
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task<int> DeleteScheduleAsync(int schedule_id) {
            Schedule? schedule = await FindScheduleAsync(schedule_id, true);
            if (schedule != null) {
                List<Schedule> schedules = await GetSchedulesAsync(schedule.HairdresserId, false);
                if (schedules.Count > 1) { // No puede haber peluquerías sin horarios
                    if (schedule.ScheduleRows != null && schedule.ScheduleRows.Count > 0) {
                        foreach (Schedule_Row srow in schedule.ScheduleRows) {
                            context.Schedule_Rows.Remove(srow);
                            await context.SaveChangesAsync();
                        }
                    } 
                    context.Schedules.Remove(schedule);
                    await context.SaveChangesAsync();
                    return 0; // OK 
                } else {
                    return 1; // Eliminación denegada
                }
            }
            return 1; // Eliminación denegada
        }

        public async Task ActivateScheduleAsync(int hairdresser_id, int schedule_id) {
            Schedule? schedule = await FindScheduleAsync(schedule_id, false);
            if (schedule != null) {
                List<Schedule> schedules = await GetSchedulesAsync(hairdresser_id, false);
                foreach (Schedule sch in schedules) {
                    if (sch.Active) { sch.Active = false; }
                }
                schedule.Active = true;
                await context.SaveChangesAsync();
            }
        }
        #endregion

        #region SCHEDULE_ROW
        public async Task<List<Schedule_Row>> GetScheduleRowsAsync(int schedule_id) {
            var consulta = from data in context.Schedule_Rows
                           where data.ScheduleId == schedule_id
                           select data;
            return await consulta.ToListAsync();
        }

        public async Task<List<Schedule_Row>> GetActiveScheduleRowsAsync(int hairdresser_id) {
            Schedule schedule = await FindActiveScheduleAsync(hairdresser_id);
            var consulta = from data in context.Schedule_Rows
                           where data.ScheduleId == schedule.ScheduleId
                           select data;
            return await consulta.ToListAsync();
        }

        public async Task<Schedule_Row?> FindScheduleRowAsync(int schedule_row_id) {
            var consulta = from data in context.Schedule_Rows
                           where data.ScheduleRowId == schedule_row_id
                           select data;
            return await consulta.FirstOrDefaultAsync();
        }

        private async Task<int> ValidateScheduleRowAsync(Schedule_Row schedule_row) {
            int schrow_compare = TimeSpan.Compare(schedule_row.Start, schedule_row.End);
            if (
                schrow_compare == 0 || schrow_compare == 1
            ) { return (int)Validates.Rango_incorrecto; }

            List<Schedule_Row> rows = await GetScheduleRowsAsync(schedule_row.ScheduleId);
            foreach (Schedule_Row row in rows) {
                if ( // Validación de duplicados. Todos los valores tienen que ser iguales
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
                ) { return (int)Validates.Duplicado; }

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
                            start_compare_start == 0 ||                                                         // Los inicios igual al rango
                            start_compare_start == 1 && start_compare_end == -1 ||                            // El inicio está entre los rangos
                            start_compare_start == 1 && (end_compare_end == 0 || end_compare_end == -1) ||    // Inicio correcto, final menor o igual al rango
                            start_compare_start == -1 && end_compare_start == 1 && end_compare_end == -1 || // Inicio correcto, final entre rangos
                            start_compare_start == -1 && (end_compare_end == 0 || end_compare_end == 1)       // Inicio correcto, final superior al rango
                        )
                    ) { return (int)Validates.Rango_Sobreescrito; }
                }

            }
            return (int)Validates.Ok;
        }

        public async Task<Response> InsertScheduleRowsAsync(int schid, TimeSpan start, TimeSpan end, bool mon, bool tue, bool wed, bool thu, bool fri, bool sat, bool sun) {
            var newid = context.Schedule_Rows.Any() ? context.Schedule_Rows.Max(s => s.ScheduleRowId) + 1 : 1;
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

            int validation = await ValidateScheduleRowAsync(schedule_row);
            if (validation == (int)Validates.Ok) { // Validación correcta
                context.Schedule_Rows.Add(schedule_row);
                await context.SaveChangesAsync();
            }
            return new Response { SatisfactoryId = newid, ResponseCode = validation };
        }

        public async Task DeleteScheduleRowsAsync(int schedule_row_id) {
            Schedule_Row? schedule_row = await FindScheduleRowAsync(schedule_row_id);
            if (schedule_row != null) {
                context.Schedule_Rows.Remove(schedule_row);
                await context.SaveChangesAsync();
            }
        }
        #endregion

        #region APPOINTMENTS
        public async Task<Appointment?> FindAppoinmentAsync(int appointment_id) {
            return await context.Appointments.FirstOrDefaultAsync(x => x.AppointmentId == appointment_id);
        }

        public async Task<List<Appointment>> GetAppointmentsByHairdresserAsync(int hairdresser_id) {
            return await context.Appointments.Where(x => x.HairdresserId == hairdresser_id).ToListAsync();
        }

        public async Task<List<Appointment>> GetAppointmentsByUserAsync(int user_id) {
            return await context.Appointments.Where(x => x.UserId == user_id).ToListAsync();
        }

        public async Task<int> InsertAppointmentAsync(int user_id, int hairdresser_id, DateTime date, TimeSpan time) {
            var newid = context.Appointments.Any() ? context.Appointments.Max(a => a.AppointmentId) + 1 : 1;
            Appointment appointment = new Appointment {
                AppointmentId = newid,
                UserId = user_id,
                HairdresserId = hairdresser_id,
                Date = date,
                Time = time,
                Status = (int)StatusAppointment.NoConfirmada
            };
            context.Appointments.Add(appointment);
            await context.SaveChangesAsync();
            return newid;
        }

        public async Task UpdateAppointmentAsync(int appointment_id, DateTime date, TimeSpan time) {
            Appointment? appointment = await FindAppoinmentAsync(appointment_id);
            if (appointment != null) {
                appointment.Date = date;
                appointment.Time = time;
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAppointmentAsync(int appointment_id) {
            Appointment? appointment = await FindAppoinmentAsync(appointment_id);
            if (appointment != null) {
                List<int> app_services = await GetAppointmentServiceAsync(appointment_id);
                foreach (int service in app_services) {
                    await DeleteAppointmentServiceAsync(appointment_id, service);
                }
                context.Appointments.Remove(appointment);
                await context.SaveChangesAsync();
            }
        }

        public async Task ApproveAppointmentAsync(int appointment_id) {
            Appointment? appointment = await FindAppoinmentAsync(appointment_id);
            if (appointment != null) {
                appointment.Status = StatusAppointment.Activa;
                await context.SaveChangesAsync();
            }
        }
        #endregion

        #region SERVICES
        public async Task<Service?> FindServiceAsync(int service_id) {
            return await context.Services.FirstOrDefaultAsync(s => s.ServiceId == service_id);
        }

        public async Task<List<Service>> GetServicesByHairdresserAsync(int hairdresser_id) {
            return await context.Services.Where(s => s.HairdresserId == hairdresser_id).ToListAsync();
        }

        public async Task<List<Service>> GetServicesByAppointmentAsync(int appointment_id) {
            List<int> app_services = await GetAppointmentServiceAsync(appointment_id);
            List<Service> services = new List<Service>();
            foreach (int app_service_id in app_services) {
                Service? newService = await FindServiceAsync(app_service_id);
                if (newService != null) { services.Add(newService); }
            }
            return services;
        }

        public async Task<List<Service>> GetServicesByIdentificationAsync(List<int> app_services) {
            List<Service> services = new List<Service>();
            foreach (int app_service_id in app_services) {
                Service? newService = await FindServiceAsync(app_service_id);
                if (newService != null) { services.Add(newService); }
            }
            return services;
        }

        public async Task<int> InsertServiceAsync(int hairdresser_id, string name, decimal price, byte duracion) {
            var newid = context.Services.Any() ? context.Services.Max(a => a.ServiceId) + 1 : 1;
            Service service = new Service {
                ServiceId = newid,
                HairdresserId = hairdresser_id,
                Name = name,
                Price = price,
                TiempoAprox = duracion
            };
            context.Services.Add(service);
            await context.SaveChangesAsync();
            return newid;
        }

        public async Task UpdateServiceAsync(int service_id, string name, decimal price, byte duracion) {
            Service? service = await FindServiceAsync(service_id);
            if (service != null) {
                service.Name = name;
                service.Price = price;
                service.TiempoAprox = duracion;
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteServiceAsync(int service_id) {
            Service? service = await FindServiceAsync(service_id);
            if (service != null) {
                context.Services.Remove(service);
                await context.SaveChangesAsync();
            }
        }
        #endregion

        #region APPOINTMENT_SERVICES
        public async Task<Appointment_Service?> FindAppointmentServiceAsync(int appointment_id, int service_id) {
            return await context.AppointmentServices.FirstOrDefaultAsync(aps => aps.AppointmentId == appointment_id && aps.ServiceId == service_id);
        }

        public async Task<List<int>> GetAppointmentServiceAsync(int appointment_id) {
            var consulta = from data in context.AppointmentServices
                           where data.AppointmentId == appointment_id
                           select data.ServiceId;
            return await consulta.ToListAsync();
        }

        public async Task<List<Appointment_Service>> GetObjectAppointmentServiceAsync(int appointment_id) {
            var consulta = from data in context.AppointmentServices
                           where data.AppointmentId == appointment_id
                           select data;
            return await consulta.ToListAsync();
        }

        public async Task InsertAppointmentServiceAsync(int appointment_id, int service_id) {
            Appointment_Service? appointmentService = await FindAppointmentServiceAsync(appointment_id, service_id);
            if (appointmentService == null) {
                Appointment_Service newAppointmentService = new Appointment_Service {
                    AppointmentId = appointment_id,
                    ServiceId = service_id
                };
                context.AppointmentServices.Add(newAppointmentService);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAppointmentServiceAsync(int appointment_id, int service_id) {
            Appointment_Service? appointmentService = await FindAppointmentServiceAsync(appointment_id, service_id);
            if (appointmentService != null) {
                context.AppointmentServices.Remove(appointmentService);
                await context.SaveChangesAsync();
            }
        }
        #endregion

    }
}
