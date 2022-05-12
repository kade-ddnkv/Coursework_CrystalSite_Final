using Coursework_CrystalSite_Final.Models;

namespace Coursework_CrystalSite_Final
{
    /// <summary>
    /// Класс для создания html-ссылок.
    /// </summary>
    public class LinkBuilder
    {
        /// <summary>
        /// Создает html-ссылку на литературу (ссылка внутри Сервиса).
        /// </summary>
        /// <param name="bookNumber"></param>
        /// <returns></returns>
        public static string CreateBookLink(int? bookNumber)
        {
            if (bookNumber.HasValue)
            {
                return $@"<a class=""bookLink"" href=""/books/id/{bookNumber.Value}"" 
onClick=""window.open('/books/id/{bookNumber.Value}', 'pagename', 'resizable,height=170,width=500'); return false;"" 
target=""_blank"">{bookNumber.Value}</a>";
            }
            else
            {
                return "NULL";
            }
        }

        /// <summary>
        /// Создает html-ссылку на соединение (ссылка внутри Сервиса).
        /// </summary>
        /// <param name="chemicalHtmlName"></param>
        /// <returns></returns>
        public static string CreateChemicalLink(string chemicalHtmlName)
        {
            return $@"<a class=""chemicalLink"" 
href=""/data-by-chemical/ch/{ChemicalModel.RemoveHtmlTags(chemicalHtmlName)}"">{chemicalHtmlName}</a>";
        }
    }
}
