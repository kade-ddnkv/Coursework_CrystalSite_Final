namespace Coursework_CrystalSite_Final.Models
{
    public class TableWithoutSyngonyModel
    {
        public List<string> ColumnNames { get; set; }

        public IEnumerable<List<object>> Rows { get; set; }
    }
}
