using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Coursework_CrystalSite_Final.Models
{
    /// <summary>
    /// Модель соединения.
    /// </summary>
    public class ChemicalModel
    {
        /// <summary>
        /// Номер соединения в БД.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Html-название соединения.
        /// </summary>
        public string HtmlName { get; set; }

        /// <summary>
        /// Убирает все html-тэги из строки.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveHtmlTags(string str)
        {
            return Regex.Replace(str, "<.*?>", string.Empty);
        }

        /// <summary>
        /// Отображение url-имени соединения в номер в БД.
        /// </summary>
        public static Dictionary<string, int> UrlNameToId = new();

        /// <summary>
        /// Отображение номера соединения в его html-имя.
        /// </summary>
        public static Dictionary<int, string> IdToHtmlName = new();

        /// <summary>
        /// Отображение url-имени соединения в его html-имя.
        /// </summary>
        public static Dictionary<string, string> UrlNameToHtmlName = new();

        /// <summary>
        /// Статический конструктор. Заполняет все отображения для соединений.
        /// </summary>
        static ChemicalModel()
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.ConnectionString);
            List<ChemicalModel> chemicals = db.Query<ChemicalModel>(
                "SELECT [Номер соединения] as Id, [Соединение] as HtmlName FROM dbo.[Соединения]").ToList();
            foreach (ChemicalModel chemical in chemicals)
            {
                string urlName = RemoveHtmlTags(chemical.HtmlName);
                UrlNameToId.Add(urlName, chemical.Id);
                IdToHtmlName.Add(chemical.Id, chemical.HtmlName);
                UrlNameToHtmlName.Add(urlName, chemical.HtmlName);
            }
        }

        /// <summary>
        /// Сравнивает системы веществ лексикографически, убирая из рассмотрения html-тэги и числа.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static int CompareChemicals(ChemicalModel first, ChemicalModel second)
        {
            string newFirst = Regex.Replace(first.HtmlName, "<.*?>", string.Empty);
            newFirst = Regex.Replace(newFirst, @"[\d]", string.Empty);
            string newSecond = Regex.Replace(second.HtmlName, "<.*?>", string.Empty);
            newSecond = Regex.Replace(newSecond, @"[\d]", string.Empty);
            return newFirst.CompareTo(newSecond);
        }
    }
}
