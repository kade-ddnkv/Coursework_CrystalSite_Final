namespace Coursework_CrystalSite_Final.Models
{
    public class QueryResultModel
    {
        public QueryResultModel(List<ChemicalModel> briefChemicalList, DynamicTableModel dynamicTable)
        {
            this.briefChemicalsList = briefChemicalList;
            this.dynamicTable = dynamicTable;
        }

        public List<ChemicalModel> briefChemicalsList { get; set; }

        public DynamicTableModel dynamicTable { get; set; }
    }
}
