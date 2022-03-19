using System.Linq;

namespace Coursework_CrystalSite_Final.Models
{
    public class DynamicTableModel
    {
        public DynamicTableModel(IEnumerable<dynamic> queryResult)
        {
            columnNames = (queryResult.First() as IDictionary<string, object>).Keys;
            rows = queryResult.Select(row => (row as IDictionary<string, object>).Values);
        }

        public IEnumerable<string> columnNames { get; set; }

        public IEnumerable<IEnumerable<object>> rows { get; set;}
    }
}
