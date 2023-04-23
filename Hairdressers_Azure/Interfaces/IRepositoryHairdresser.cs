using Hairdressers_Azure.Models;

namespace Hairdressers_Azure.Interfaces {
    public interface IRepositoryHairdresser {

        #region ENUMS
        public enum ResponseCodes : int { // Respuestas genéricas de la clase Response
            OK = 200,
            Failed = 412,
            NotAuthorized = 401
        }

        public enum ResponseErrorCodes : int { // Respuestas específicas de los errores de la clase Response
            GeneralError = 0,
            RecordNotFound = 1, 
            RecordExisting = 2, 
            BlockOneAdmin = 3,
            Duplicate = 4,
            OverwriteRange = 5,
            IncorrectRange = 6
        }
        #endregion

        #region CREDENTIALS
        Task<User?> LogInAsync(string email, string password);
        #endregion

        #region TOKENS
        string GenerateToken();
        Task<bool> UserAssignTokenAsync(int user_id, string token);
        Task<bool> UserValidateTokenAsync(int user_id, string token);
        Task<bool> HairdresserValidateTokenAsync(int hairdresser_id, string token);
        #endregion

        #region ADMINS
        Task<Admin?> FindAdminAsync(int hairdresser_id, int user_id);
        Task<List<Admin>> GetAdminsAsync(int hairdresser_id);
        Task<bool> AdminExistAsync(int hairdresser_id, int user_id);
        Task<bool> CompareAdminRoleAsync(int user_id_me, int user_id_other);
        Task<Response> InsertAdminAsync(int hairdresser_id, int user_id, AdminRole role);
        Task<Response> UpdateAdminAsync(int hairdresser_id, int user_id, AdminRole role);
        Task<Response> DeleteAdminAsync(int hairdresser_id, int user_id);
        #endregion

        #region USERS
        Task<bool> UserIsAdminAsync(int user_id);
        Task<bool> EmailExistAscync(string email);
        Task<User?> FindUserAsync(int user_id);
        Task<User?> InsertUserAsync(string password, string name, string lastname, string phone, string email, bool econfirmed);
        Task<Response> UpdateUserAsync(int user_id, string name, string lastname, string phone, string email);
        Task<Response> ValidateEmailAsync(int user_id);
        #endregion

        #region HAIRDRESSERS
        Task<Hairdresser?> FindHairdresserAsync(int hairdresser_id);
        Task<List<string>> GetHairdresserEmailsAsync(int hairdresser_id);
        Task<List<Hairdresser>> GetHairdressersAsync();
        Task<List<Hairdresser>> GetHairdressersByUserAsync(int user_id);
        Task<List<Hairdresser>> GetHairdressersByFilterNameAsync(string filterName);
        Task<Response> InsertHairdresserAsync(string name, string phone, string address, int postal_code, int user_id);
        Task<Response> UpdateHairdresserAsync(int hairdresser_id, string name, string phone, string address, int postal_code);
        Task<Response> DeleteHairdresserAsync(int hairdresser_id);
        #endregion

        #region SCHEDULES
        Task<Schedule?> FindScheduleAsync(int schedule_id, bool getRows);
        Task<Schedule?> FindActiveScheduleAsync(int hairdresser_id, bool getRows);
        Task<List<string>> GetNameSchedulesAsync(int hairdresser_id);
        Task<List<Schedule>> GetSchedulesAsync(int hairdresser_id, bool getRows);
        Task<Response> InsertScheduleAsync(int hairdresser_id, string name, bool active);
        Task<Response> UpdateScheduleAsync(int schedule_id, int hairdresser_id, string name, bool active);
        Task<Response> DeleteScheduleAsync(int schedule_id);
        Task<Response> ActivateScheduleAsync(int hairdresser_id, int schedule_id);
        #endregion

        #region SCHEDULE_ROWS
        Task<Schedule_Row?> FindScheduleRowAsync(int schedule_row_id);
        Task<List<Schedule_Row>> GetScheduleRowsAsync(int schedule_id);
        Task<List<Schedule_Row>> GetActiveScheduleRowsAsync(int hairdresser_id);
        Task<Response> InsertScheduleRowsAsync(int schid, TimeSpan start, TimeSpan end, bool mon, bool tue, bool wed, bool thu, bool fri, bool sat, bool sun);
        Task<Response> DeleteScheduleRowsAsync(int schedule_row_id);
        #endregion

        #region APPOINTMENTS
        Task<Appointment?> FindAppoinmentAsync(int appointment_id);
        Task<List<Appointment>> GetAppointmentsByUserAsync(int user_id);
        Task<List<Appointment>> GetAppointmentsByHairdresserAsync(int hairdresser_id);
        Task<Response> InsertAppointmentAsync(int user_id, int hairdresser_id, DateTime date, TimeSpan time);
        Task<Response> UpdateAppointmentAsync(int appointment_id, DateTime date, TimeSpan time);
        Task<Response> DeleteAppointmentAsync(int appointment_id);
        Task<Response> ApproveAppointmentAsync(int appointment_id);
        #endregion

        #region SERVICES
        Task<Service?> FindServiceAsync(int service_id);
        Task<List<Service>> GetServicesByHairdresserAsync(int hairdresser_id);
        Task<List<Service>> GetServicesByAppointmentAsync(int appoinment_id);
        Task<List<Service>> GetServicesByIdentificationAsync(List<int> app_services);
        Task<Response> InsertServiceAsync(int hairdresser_id, string name, decimal price, byte duration);
        Task<Response> UpdateServiceAsync(int service_id, string name, decimal price, byte duration);
        Task<Response> DeleteServiceAsync(int service_id);
        #endregion

        #region APPOINTMENT_SERVICES
        Task<Appointment_Service?> FindAppointmentServiceAsync(int appointment_id, int service_id);
        Task<List<int>> GetAppointmentServicesIDsAsync(int appointment_id);
        Task<List<Appointment_Service>> GetAppointmentServicesAsync(int appointment_id);
        Task<Response> InsertAppointmentServiceAsync(int appointment_id, int service_id);
        Task<Response> DeleteAppointmentServiceAsync(int appointment_id, int service_id);
        #endregion

    }
}
