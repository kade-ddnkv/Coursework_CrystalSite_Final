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
        public const string ConnectionString = "Server=tcp:crystal.database.windows.net,1433;Initial Catalog=Crystal;Persist Security Info=False;User ID=rootcrystal;Password=73u$7$BmC7VW;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
    }
}
