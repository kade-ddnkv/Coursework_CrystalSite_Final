using Coursework_CrystalSite_Final.Models;

namespace Coursework_CrystalSite_Final
{
    public class LinkBuilder
    {
        public static string CreateBookLink(int bookNumber)
        {
            return $@"<a class=""bookLink"" href=""/books/id/{bookNumber}"" 
onClick=""window.open('/books/id/{bookNumber}', 'pagename', 'resizable,height=150,width=500'); return false;"" 
target=""_blank"">{bookNumber}</a>";
        }

        public static string CreateChemicalLink(string chemicalHtmlName)
        {
            return $@"<a class=""chemicalLink"" 
href=""/data-by-chemical/ch/{ChemicalModel.RemoveHtmlTags(chemicalHtmlName)}"">{chemicalHtmlName}</a>";
        }
    }
}
