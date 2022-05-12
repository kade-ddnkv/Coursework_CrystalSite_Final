namespace Coursework_CrystalSite_Final.Models
{
    /// <summary>
    /// Модель таблицы для вывода свойства определенного соединения.
    /// </summary>
    public class PropertyTableModel
    {
        /// <summary>
        /// Html-название соединения.
        /// </summary>
        public string ChemicalName { get; set; }

        /// <summary>
        /// Название свойства.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Таблица свойства соединения. (В случае, если для свойства есть различия по сингонии).
        /// </summary>
        public TableWithSyngonyModel TableWithSyngonyModel { get; set; }

        /// <summary>
        /// Таблица свойства соединения. (В случае, если для свойства нет различий по сингонии).
        /// </summary>
        public TableWithoutSyngonyModel TableWithoutSyngonyModel { get; set; }
    }
}
