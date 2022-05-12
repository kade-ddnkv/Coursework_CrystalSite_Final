namespace Coursework_CrystalSite_Final.Models
{
    /// <summary>
    /// Модель сингонии с таблицей (для свойства, поддерживающего разделение по сингонии).
    /// </summary>
    public class SyngonyModel
    {
        /// <summary>
        /// Полное название сингонии.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Значения таблицы по строкам (<td>).
        /// </summary>
        public IEnumerable<IEnumerable<object>> Rows { get; set; }
    }
}
