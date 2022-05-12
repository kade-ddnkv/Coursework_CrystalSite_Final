namespace Coursework_CrystalSite_Final.Models
{
    /// <summary>
    /// Модель таблиц, в случае, если свойство соединения поддерживает различия по сингонии.
    /// </summary>
    public class TableWithSyngonyModel
    {
        /// <summary>
        /// Названия столбцов таблицы (<th>).
        /// </summary>
        public List<string> ColumnNames { get; set; }

        /// <summary>
        /// Список таблиц по различным сингониям.
        /// </summary>
        public List<SyngonyModel> SyngGroups { get; set; }
    }
}
