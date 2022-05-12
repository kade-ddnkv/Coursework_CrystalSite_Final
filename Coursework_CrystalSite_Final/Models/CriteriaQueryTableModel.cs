using System.Linq;

namespace Coursework_CrystalSite_Final.Models
{
    /// <summary>
    /// Модель динамически генерируемой таблицы в результате многокритериального запроса.
    /// </summary>
    public class CriteriaQueryTableModel
    {
        /// <summary>
        /// Конструктор из запроса в БД.
        /// </summary>
        /// <param name="queryResult"></param>
        /// <param name="isOneCriteria"></param>
        public CriteriaQueryTableModel(IEnumerable<dynamic> queryResult, bool isOneCriteria)
        {
            // Если в запросе есть хотя бы один элемент.
            if (queryResult.FirstOrDefault() != null)
            {
                ColumnNames = (queryResult.First() as IDictionary<string, dynamic>).Keys.ToList();
                Rows = queryResult.Select(row => (row as IDictionary<string, dynamic>).Values.ToList()).ToList();
                if (isOneCriteria)
                {
                    // Если критерий только один, то выводится вся информация из одной таблицы.
                    // Поэтому нужно убрать второй столбец - номер соединения.
                    ColumnNames.RemoveAt(1);
                    Rows.ForEach(row => row.RemoveAt(1));
                    // К последнему столбцу нужно привязать ссылки на литуратуру.
                    Rows.ForEach(row => row[row.Count - 1] = LinkBuilder.CreateBookLink(row.Last()));
                }
                // К первому столбцу (название соединения) нужно привязать ссылки на соотв. соединения.
                Rows.ForEach(row => row[0] = LinkBuilder.CreateChemicalLink(row[0]));
            }
        }

        /// <summary>
        /// Названия столбцов в таблице.
        /// </summary>
        public List<string> ColumnNames { get; set; }

        /// <summary>
        /// Значения таблицы по строкам.
        /// </summary>
        public List<List<dynamic>> Rows { get; set; }
    }
}
