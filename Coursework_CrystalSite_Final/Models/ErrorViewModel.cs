namespace Coursework_CrystalSite_Final.Models
{
    /// <summary>
    /// Модель для вывода ошибки.
    /// </summary>
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}