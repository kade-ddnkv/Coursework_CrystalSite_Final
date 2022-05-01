using System.Linq;

namespace Coursework_CrystalSite_Final.Models
{
    public class DynamicTableModel
    {
        public DynamicTableModel(IEnumerable<dynamic> queryResult, bool isOneCriteria)
        {
            columnNames = (queryResult.First() as IDictionary<string, object>).Keys.ToList();
            if (isOneCriteria)
            {
                columnNames.RemoveAt(1);
                rows = queryResult.Select(row => (row as IDictionary<string, object>).Values.ToList()).ToList();
                rows.ForEach(row => row.RemoveAt(1));
            }
            else
            {
                rows = queryResult.Select(row => (row as IDictionary<string, object>).Values.ToList()).ToList();
            }
        }

        public List<string> columnNames { get; set; }

        public List<List<object>> rows { get; set; }
    }
}
