using Azure.Security.KeyVault.Secrets;
using Hairdressers_Azure.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Hairdressers_Azure.Controllers {
    public class SecretController : Controller {

        private SecretClient secretClient;

        public SecretController(SecretClient secretClient) {
            this.secretClient = secretClient;
        }

        [AuthorizeUsers] [HttpGet]
        public async Task<IActionResult> GetSecret(string secretName) {
            KeyVaultSecret secret = await secretClient.GetSecretAsync(secretName);
            return Json(secret.Value);
        }

    }
}
