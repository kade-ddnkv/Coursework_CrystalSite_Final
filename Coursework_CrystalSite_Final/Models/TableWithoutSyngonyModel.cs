namespace Coursework_CrystalSite_Final.Models
{
    /// <summary>
    /// Модель таблиц, в случае, если свойство соединения не поддерживает различия по сингонии.
    /// </summary>
    public class TableWithoutSyngonyModel
    {
        /// <summary>
        /// Названия столбцов таблицы (<th>).
        /// </summary>
        public List<string> ColumnNames { get; set; }

        /// <summary>
        /// Значения таблицы по строкам (<td>).
        /// </summary>
        public IEnumerable<List<object>> Rows { get; set; }
    }
}
