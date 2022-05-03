namespace Coursework_CrystalSite_Final.Models
{
    public class SyngonyModel
    {
        public string Name { get; set; }

        public IEnumerable<IEnumerable<object>> Rows { get; set; }
    }
}
