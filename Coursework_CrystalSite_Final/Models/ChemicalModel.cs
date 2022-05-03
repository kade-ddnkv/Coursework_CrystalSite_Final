using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Coursework_CrystalSite_Final.Models
{
    public class ChemicalModel
    {
        public int Id { get; set; }

        public string HtmlName { get; set; }

        public static string RemoveHtmlTags(string str)
        {
            return Regex.Replace(str, "<.*?>", string.Empty);
        }

        public static Dictionary<string, int> UrlNameToId = new();
        public static Dictionary<int, string> IdToHtmlName = new();
        public static Dictionary<string, string> UrlNameToHtmlName = new();

        static ChemicalModel()
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.connectionString);
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
