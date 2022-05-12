using Microsoft.AspNetCore.Html;

namespace Coursework_CrystalSite_Final.Models
{
    /// <summary>
    /// Вспомогательная модель для создания критерия для многокритериального поиска: 
    /// один параметр находится в определенном интервале.
    /// </summary>
    public class _OneRangeCriteriaModel
    {
        /// <summary>
        /// Название критерия.
        /// </summary>
        public string CurrentPropertyName { get; set; }

        /// <summary>
        /// Надпись перед вводом значений в интервал.
        /// </summary>
        public string RangeLabel { get; set; }

        /// <summary>
        /// Символ, находящийся внутри интервала.
        /// </summary>
        public string RangeSymbol { get; set; }

        /// <summary>
        /// Название параметра (атрибут name для input).
        /// </summary>
        public string RangeName { get; set; }
    }
}
