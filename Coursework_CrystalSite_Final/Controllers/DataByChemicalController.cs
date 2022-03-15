using Coursework_CrystalSite_Final.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Coursework_CrystalSite_Final.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataByChemicalController : Controller
    {
        [HttpGet("index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("get-chemicals-by-elements")]
        public IActionResult GetChemicalsByName([FromQuery] string elements)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.connectionString);
            string[] chemStrings = elements.Split('-', StringSplitOptions.RemoveEmptyEntries);
            IEnumerable<ChemicalModel> chemicals = Enumerable.Empty<ChemicalModel>();
            if (chemStrings.Length != 0)
            {
                string query = @"SELECT HeadClue as Id, System as HtmlName FROM dbo._HeadTablConv ";
                string filter = "WHERE " + string.Join(" AND ", chemStrings.Select(chem => $"Help LIKE '%{chem}%'"));
                chemicals = db.Query<ChemicalModel>(
                    query + filter
                    , new { chemicalName = elements });
            }
            return PartialView("_FoundChemicalsPartialView", chemicals);
        }

        [HttpGet("get-properties-by-chemical")]
        public IActionResult GetChemicalProperties([FromQuery(Name = "chemical_id")] int chemicalId)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.connectionString);
            IEnumerable<PropertyModel> propertiesExistance = db.Query<PropertyModel>(@"
                    SELECT dbo.Properties.NOMPROP AS Id
	                    ,NAZVPROP AS NameRus
	                    ,TableName
	                    ,CASE 
		                    WHEN HeadClue IS NULL
			                    THEN 'false'
		                    ELSE 'true'
		                    END AS Existing
                    FROM Crystal.dbo.Properties
                    LEFT JOIN (
	                    SELECT *
	                    FROM dbo._DBContentConv
	                    WHERE HeadClue = 7
	                    ) AS existing_props ON dbo.Properties.NOMPROP = existing_props.NOMPROP;
                ", new { chemicalId = chemicalId });
            return PartialView("_PropertiesPartialView", propertiesExistance);
        }

        [NonAction]
        public (string, IEnumerable<dynamic>) GetPropertyValuesFromViews(int chemicalId, int propertyId)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.connectionString);
            string nameOfPropertyRus = db.QuerySingleOrDefault<dynamic>(
                "SELECT NAZVPROP as ViewNameRus FROM dbo.Properties WHERE NOMPROP = @propertyId"
                , new { propertyId }).ViewNameRus;
            return (nameOfPropertyRus, db.Query<dynamic>(
                $"SELECT * FROM dbo.[{nameOfPropertyRus}] WHERE [Номер соединения] = @chemicalId"
                , new { chemicalId }));
        }

        [NonAction]
        public (string, IEnumerable<dynamic>) GetPropertyValuesFromInnerTables(int chemicalId, int propertyId)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.connectionString);
            string nameOfPropertyEn = db.QuerySingleOrDefault<dynamic>(
                "SELECT TableName AS TableNameEn FROM dbo.Properties WHERE NOMPROP = @propertyId"
                , new { propertyId }).TableNameEn;
            return (nameOfPropertyEn, db.Query<dynamic>(
                $"SELECT * FROM dbo.[{nameOfPropertyEn}] WHERE HeadClue = @chemicalId"
                , new { chemicalId }));
        }

        [HttpGet("get-property-table")]
        public IActionResult GetPropertyValues([FromQuery(Name = "chemical_id")] int chemicalId, [FromQuery(Name = "property_id")] int propertyId)
        {
            string propertyName;
            List<string> columnNames;
            IEnumerable<List<object>> rows;
            IEnumerable<dynamic> queryResult;
            try
            {
                (propertyName, queryResult) = GetPropertyValuesFromViews(chemicalId, propertyId);
            }
            catch (SqlException)
            {
                throw new Exception("Special property that doesn't have a russian view. Rememeber it and hadle this property in a different way.");
                (propertyName, queryResult) = GetPropertyValuesFromInnerTables(chemicalId, propertyId);
            }
            columnNames = ((IDictionary<string, object>)queryResult.FirstOrDefault())?.Keys?.ToList();
            rows = queryResult.Select(x => ((IDictionary<string, dynamic>)x).Values.ToList());

            if (columnNames.Contains("Сингония"))
            {
                int syngColumnIndex = columnNames.IndexOf("Сингония");
                var syngGroups = rows
                    .GroupBy(row => row[syngColumnIndex])
                    .Select(group => new { name = group.Key, rows = group as IEnumerable<IEnumerable<object>> });
                return PartialView("_SyngonyPartialView", new { propertyName, columnNames, syngGroups });
            }

            return PartialView("_SyngonyPartialView", new { propertyName, columnNames, rows });
        }
    }
}
