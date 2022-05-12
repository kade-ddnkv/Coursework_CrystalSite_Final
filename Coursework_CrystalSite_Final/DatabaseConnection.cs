namespace Coursework_CrystalSite_Final
{
    /// <summary>
    /// Класс, содержащий константы для связи с БД.
    /// </summary>
    public class DatabaseConnection
    {
        /// <summary>
        /// Строка для подключения к базе данных. Например, через Dapper.
        /// </summary>
        public const string ConnectionString = "Server=localhost; Database=Crystal; Integrated Security=True; TrustServerCertificate=True;";
    }
}
