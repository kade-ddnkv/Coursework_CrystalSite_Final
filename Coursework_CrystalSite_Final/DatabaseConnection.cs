namespace Coursework_CrystalSite_Final
{
    /// <summary>
    /// Класс, содержащий константы для связи с БД.
    /// </summary>
    public class DatabaseConnection
    {
        /// <summary>
        /// Строка для подключения к локальной базе данных. Например, через Dapper.
        /// </summary>
        public const string ConnectionStringObsolete = "Server=localhost; Database=Crystal; Integrated Security=True; TrustServerCertificate=True;";

        /// <summary>
        /// Строка подключения к базе данных в Azure.
        /// </summary>
        public const string ConnectionString = "[ДАННЫЕ УДАЛЕНЫ]";
    }
}
