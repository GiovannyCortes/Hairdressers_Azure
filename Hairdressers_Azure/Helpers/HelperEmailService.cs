using Azure.Security.KeyVault.Secrets;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;

namespace Hairdressers_Azure.Helpers {
    public class HelperEmailService {

        private MediaTypeWithQualityHeaderValue Header;
        private string image_url;
        private string cutandgo_website;
        private string emailscutandgo;

        public HelperEmailService(SecretClient secretClient) {
            this.Header = new MediaTypeWithQualityHeaderValue("application/json");
            this.image_url = GetSecretAsync(secretClient, "cutandgocardimage").Result;
            this.cutandgo_website = GetSecretAsync(secretClient, "cutandgowebsite").Result;
            this.emailscutandgo = GetSecretAsync(secretClient, "emailscutandgo").Result;
        }

        private static async Task<string> GetSecretAsync(SecretClient secretClient, string secretName) {
            KeyVaultSecret keyVaultSecret = await secretClient.GetSecretAsync(secretName);
            return keyVaultSecret.Value;
        }

        public async Task SendMailAsync(string destinatario, string asunto, string mensaje) {
            var emailModel = new {
                email = destinatario,
                asunto = asunto,
                mensaje = mensaje
            };
            using (HttpClient client = new HttpClient()) {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);

                string json = JsonConvert.SerializeObject(emailModel);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PostAsync(this.emailscutandgo, content);
            }
        }

        public async Task SendTemplateVerificationEmailAsync(string destinatario, string cliente, string token) {
            string mensaje = $@"
                <html>
                <head>
                    <meta charset='UTF-8'>
                </head>
                <body>
                    <h2 style='text-aling:center;'>
                        Verificación de cuenta
                    </h2>
                    <p style='font-size: 1em;'>
                        Estimado/a <strong>{cliente}</strong><br>

                        Para poder verificar su correo electrónico en la aplicación Cut&Go, haga clic en el siguiente enlace: <br> <br>

                        <center><a href='{this.cutandgo_website}/User/ValidateEmail?token={token}' style='
                            text-decoration: none;
                            background-color: #415073;
                            color: white;
                            padding: 10px;
                            border-radius: 25px;
                            font-weight: 700;
                            width: 90%;
                            display: inline-block;
                        '>
                            Verificar email
                        </a></center> <br> <br>

                        Después de hacer clic en el enlace, serás redirigido a nuestra página web. <br>
                        Una vez allí, puedes iniciar sesión con las credenciales que proporcionaste al registrarte. <br> <br>

                        Si no has creado una cuenta en Cut&Go, puedes ignorar este correo electrónico. <br> <br>
                        Si tienes alguna pregunta o problema, no dudes en contactarnos. <br> <br>

                        Atentamente, <br>
                        El equipo de Cut&Go
                    </p> <br>
                    <center><img src='{this.image_url}' style='max-height: 150px;'/></center>
                </body>
                </html>";

            await SendMailAsync(destinatario, "Cut&Go: Verificación de cuenta", mensaje);
        }

        public async Task SendConfirmationAppointment(int appointmentId, int userId, string destinatario, string cliente, string fecha, string hora) {
            string mensaje = $@"
                <html>
                <head>
                    <meta charset='UTF-8'>
                </head>
                <body>
                    <h2 style='text-aling:center;'>
                        ¡Cita confirmada {cliente}!
                    </h2>
                    <p>
                        Desde <strong>Cut&Go</strong> le informamos que su cita para el día 
                        <strong>{fecha}</strong> a las <strong>{hora}</strong> ha sido confirmada
                        por parte del establecimiento. Su identificador es el #US{userId}AP{appointmentId}
                    </p>
                    <p>Una vez más le damos las gracias por confiar en nuestro servicio y le informamos de que
                       si tiene alguna duda por favor no dude en contactarnos: cutandgo.app@gmail.com</p>
                    <center><img src='{this.image_url}' style='max-height: 150px;'/></center>
                </body>
                </html>";
            await SendMailAsync(destinatario, "Cut&Go: Confirmación de cita", mensaje);
        }

        public async Task SendTemplateRequestAppointment
            (List<string> destinatarios, string cliente, string email, string fecha, string hora,
            List<string> servicios, decimal coste, string token, int hairdresser_id, int appointment_id) {

            string services = "";
            foreach (string servicio in servicios) {
                services += "<li>";
                services += servicio;
                services += "</li>";
            }

            string mensaje = $@"
                <html>
                <head>
                    <meta charset='UTF-8'>
                </head>
                <body>
                    <h2 style='text-aling:center;'>
                        Solicitud de cita
                    </h2>
                    <p>
                        Usuari@ <strong>{cliente}</strong> solicita una cita para el día 
                        <strong>{fecha}</strong> a las <strong>{hora}</strong>
                        para realizar los siguientes servicios:
                    </p>
                    <ul>{services}</ul>
                    <p>Coste Total: {coste} €</p>
                    <p>Puede confirmar la cita pulsando el botón de más abajo, desde la aplicación o contactando directamente con el usuario</p> <br>
                    <center>
                        <a href='{this.cutandgo_website}/Appointments/AppointmentConfirm?token={token}&hid={hairdresser_id}&apid={appointment_id}' style='
                            text-decoration: none;
                            background-color: #415073;
                            color: white;
                            padding: 10px;
                            margin-inline: 10px;
                            border-radius: 25px;
                            font-weight: 700;
                            width: 80%;
                            display: inline-block;
                        '>
                            Aceptar
                        </a>
                    </center> <br> 
                    <p>Datos de contacto: {email}</p>
                    <center><img src='{this.image_url}' style='max-height: 150px;'/></center>
                </body>
                </html>";

            foreach (string des in destinatarios) {
                await SendMailAsync(des, "Cut&Go: Solicitud de cita", mensaje);
            }
        }

    }
}