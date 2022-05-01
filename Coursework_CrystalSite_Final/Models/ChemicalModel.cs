using System.Text.RegularExpressions;

namespace Coursework_CrystalSite_Final.Models
{
    public class ChemicalModel
    {
        public int Id { get; set; }

        public string HtmlName { get; set; }

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
