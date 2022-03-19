namespace Coursework_CrystalSite_Final.Models
{
    public class QueryResultModel
    {
        public QueryResultModel(IEnumerable<object> briefChemicalList, DynamicTableModel dynamicTable)
        {
            this.briefChemicalList = briefChemicalList;
            this.dynamicTable = dynamicTable;
        }

        public IEnumerable<object> briefChemicalList { get; set; }

        public DynamicTableModel dynamicTable { get; set; }
    }
}
