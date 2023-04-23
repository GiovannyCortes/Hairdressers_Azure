namespace Hairdressers_Azure.Models {
    public class Response {

        public int ResponseCode { get; set; }       // Código genérico de respuesta. Se basa en la enumeración ResponseCodes
        public int SatisfactoryId { get; set; }     // Identificador de salida del record que se ha creado/modificado satisfactoriamente
        public int ErrorCode { get; set; }          // Código específico del error. Se basa en la enumeración ResponseErrorCodes
        public string? ErrorMessage { get; set; }   // Información adicional acerca del error obtenido

    }
}
