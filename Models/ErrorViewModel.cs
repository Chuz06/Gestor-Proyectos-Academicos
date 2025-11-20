namespace Gestor_Proyectos_Academicos.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        // hala

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
