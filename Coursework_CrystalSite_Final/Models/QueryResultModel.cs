namespace Coursework_CrystalSite_Final.Models
{
    /// <summary>
    /// Модель результата многокритериального поиска соединений.
    /// </summary>
    public class QueryResultModel
    {
        /// <summary>
        /// Стандартный конструктор.
        /// </summary>
        /// <param name="briefChemicalList"></param>
        /// <param name="dynamicTable"></param>
        public QueryResultModel(List<ChemicalModel> briefChemicalList, CriteriaQueryTableModel dynamicTable)
        {
            BriefChemicalList = briefChemicalList;
            DynamicTable = dynamicTable;
        }

        /// <summary>
        /// Краткий список всех соединений, подходящих под многокритериальный запрос.
        /// </summary>
        public List<ChemicalModel> BriefChemicalList { get; set; }

        /// <summary>
        /// Полная таблица всех наблюдений, подходящих под многокритериальный запрос.
        /// </summary>
        public CriteriaQueryTableModel DynamicTable { get; set; }
    }
}
