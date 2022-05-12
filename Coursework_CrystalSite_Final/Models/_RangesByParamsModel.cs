namespace Coursework_CrystalSite_Final.Models
{
    /// <summary>
    /// Вспомогательная модель для создания критерия для многокритериального поиска: 
    /// несколько параметров по выбору, для каждого пользователь выбирает интервал.
    /// </summary>
    public class _RangesByParamsModel
    {
        /// <summary>
        /// Название критерия.
        /// </summary>
        public string CurrentPropertyName { get; set; }

        /// <summary>
        /// Количество параметров на выбор.
        /// </summary>
        public int NumberOfParams { get; set; }

        /// <summary>
        /// Список с надписями перед вводом значений в интервал.
        /// </summary>
        public List<string> RangeLabels { get; set; }

        /// <summary>
        /// Список с символами, находящимися внутри интервала.
        /// </summary>
        public List<string> RangeSymbols { get; set; }

        /// <summary>
        /// Список с названиями параметров (атрибуты name для input).
        /// </summary>
        public List<string> RangeNames { get; set; }
    }
}
