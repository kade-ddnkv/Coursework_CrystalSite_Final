namespace Coursework_CrystalSite_Final.Models
{
    public class QueryResultModel
    {
        public QueryResultModel(List<ChemicalModel> briefChemicalList, CriteriaQueryTableModel dynamicTable)
        {
            this.briefChemicalsList = briefChemicalList;
            this.dynamicTable = dynamicTable;
        }

        public List<ChemicalModel> briefChemicalsList { get; set; }

        public CriteriaQueryTableModel dynamicTable { get; set; }
    }
}
