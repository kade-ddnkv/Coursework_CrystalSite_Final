using Coursework_CrystalSite_Final.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Coursework_CrystalSite_Final.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataByChemicalController : Controller
    {
        private Dictionary<string, string> syngonyNamesMapper = new()
        {
            { "к", "кубическая" },
            { "м", "моноклинная" },
            { "г", "гексагональная" },
            { "т", "тетрагональная" },
            { "тр", "триклинная" },
            { "тг", "тригональная" },
            { "р", "орторомбическая" },
            { "рэ", "ромбоэдрическая" }
        };

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
            List<ChemicalModel> chemicals = new();
            if (chemStrings.Length != 0)
            {
                string query = @"SELECT HeadClue as Id, System as HtmlName FROM dbo._HeadTablConv ";
                string filter = "WHERE " + string.Join(" AND ", chemStrings.Select(chem => $"Help LIKE '%{chem}%'"));
                chemicals = db.Query<ChemicalModel>(query + filter).ToList();
            }
            chemicals.Sort(ChemicalModel.CompareChemicals);
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
	                    WHERE HeadClue = @chemicalId
	                    ) AS existing_props ON dbo.Properties.NOMPROP = existing_props.NOMPROP;
                ", new { chemicalId = chemicalId });
            return PartialView("_PropertiesPartialView", propertiesExistance);
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

        [NonAction]
        public string FixNameOfPropertyToView(string nameOfPropertyRus)
        {
            switch (nameOfPropertyRus)
            {
                case "Акустооптические свойства":
                    return "Акустооптическая добротность";

                default:
                    return nameOfPropertyRus;
            }
        }

        [NonAction]
        public (string, IEnumerable<dynamic>) GetPropertyValuesFromViews(int chemicalId, int propertyId)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.connectionString);
            string nameOfPropertyRus = db.QuerySingleOrDefault<dynamic>(
                "SELECT NAZVPROP as ViewNameRus FROM dbo.Properties WHERE NOMPROP = @propertyId"
                , new { propertyId }).ViewNameRus;
            nameOfPropertyRus = FixNameOfPropertyToView(nameOfPropertyRus);
            return (nameOfPropertyRus, db.Query<dynamic>(
                $"SELECT * FROM dbo.[{nameOfPropertyRus}] WHERE [Номер соединения] = @chemicalId"
                , new { chemicalId }));
        }

        [NonAction]
        public dynamic GetAnalyticalReview(int chemicalId)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.connectionString);
            return db.QueryFirstOrDefault<dynamic>(
            $"SELECT [Аналитический обзор] as AnalyticalReview FROM dbo.[Аналитический обзор] WHERE [Номер соединения] = @chemicalId"
            , new { chemicalId });
        }

        [HttpGet("get-chemical-formula")]
        public string GetChemicalFormula([FromQuery(Name = "chemical_id")] int chemicalId)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.connectionString);
            return db.QueryFirstOrDefault<dynamic>(
            @"SELECT HeadClue as Id, System as HtmlName FROM dbo._HeadTablConv WHERE HeadClue = @chemicalId"
            , new { chemicalId }).HtmlName;
        }

        [NonAction]
        public string GetPropertyNameRus(int propertyId)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.connectionString);
            return db.QuerySingleOrDefault<dynamic>(
                "SELECT NAZVPROP as ViewNameRus FROM dbo.Properties WHERE NOMPROP = @propertyId"
                , new { propertyId }).ViewNameRus;
        }

        [NonAction]
        public dynamic GetPropertyValuesByViewNameWithSyngony(int chemicalId, string propertyName)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.connectionString);
            var queryResult = db.Query<dynamic>(
                $"SELECT * FROM dbo.[{propertyName}] WHERE [Номер соединения] = @chemicalId"
                , new { chemicalId });
            List<string> columnNames = ((IDictionary<string, object>)queryResult.FirstOrDefault()).Keys.Skip(1).ToList();
            IEnumerable<List<object>> rows = queryResult.Select(x => ((IDictionary<string, dynamic>)x).Values.Skip(1).ToList());

            int syngColumnIndex = columnNames.IndexOf("Сингония");
            var syngGroups = rows
                .GroupBy(row => row[syngColumnIndex])
                .Select(group => new { name = syngonyNamesMapper[group.Key as string], rows = group.Select(row => row.Skip(1)) })
                .ToList();
            return new { propertyName, columnNames = columnNames.Skip(1).ToList(), syngGroups };
        }

        [NonAction]
        public dynamic GetNonLinearOpticalPropertiesValues(int chemicalId)
        {
            var nonLinearOpticalCoefficients = GetPropertyValuesByViewNameWithSyngony(chemicalId, "Нелинейные оптические коэффициенты");
            var componentsOfTheMillerTensor = GetPropertyValuesByViewNameWithSyngony(chemicalId, "Компоненты тензора Миллера (10-2 м/Кл)");
            return new { nonLinearOpticalCoefficients, componentsOfTheMillerTensor };
        }

        [NonAction]
        public dynamic GetLiterature(int chemicalId)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.connectionString);
            return db.Query<BookModel>(
                @"SELECT litr.[Номер ссылки] as BookNumber
	                , [Ф.И.О. авторов] as Authors
	                , [Выходные данные] as BookNameAndPages
	                , [Название статьи/монографии] as ArticleName
	                , DOI as DoiLink
                FROM dbo.[Литературные ссылки] as litr
                JOIN dbo.[Библиографические ссылки] as bibl ON litr.[Номер ссылки] = bibl.[Номер ссылки]
                WHERE [Номер соединения] = @chemicalId"
                , new { chemicalId });
        }

        [HttpGet("get-property-table")]
        public IActionResult GetPropertyValues([FromQuery(Name = "chemical_id")] int chemicalId, [FromQuery(Name = "property_id")] int propertyId)
        {

            // Особые свойства обрабатываются отдельно.
            switch (propertyId)
            {
                case 1:
                    // "Аналитический обзор"
                    return PartialView("_AnalyticalReviewPartialView", GetAnalyticalReview(chemicalId));
                case 4:
                    // "Состав соединения"
                    return PartialView("_ChemicalFormulaPartialView", GetChemicalFormula(chemicalId));
                case 25:
                    // "Нелинейные оптические свойства"
                    return PartialView("_NonLinearOpticalPropertiesPartialView", GetNonLinearOpticalPropertiesValues(chemicalId));
                case 29:
                    // "Литература"
                    return PartialView("_LiteraturePartialView", GetLiterature(chemicalId));
            }

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
                throw new NotImplementedException("Special property that doesn't have a russian view. Rememeber it and hadle this property in a different way.");
                (propertyName, queryResult) = GetPropertyValuesFromInnerTables(chemicalId, propertyId);
            }
            columnNames = ((IDictionary<string, object>)queryResult.FirstOrDefault())?.Keys?.Skip(1)?.ToList();
            rows = queryResult.Select(x => ((IDictionary<string, dynamic>)x).Values.Skip(1).ToList());

            if (columnNames.Contains("Сингония"))
            {
                int syngColumnIndex = columnNames.IndexOf("Сингония");
                var syngGroups = rows
                    .GroupBy(row => row[syngColumnIndex])
                    .Select(group => new { name = syngonyNamesMapper[group.Key as string], rows = group.Select(row => row.Skip(1)) })
                    .ToList();
                return PartialView("_SyngonyPartialView", new { propertyName, columnNames = columnNames.Skip(1).ToList(), syngGroups });
            }

            return PartialView("_SyngonyPartialView", new { propertyName, columnNames, rows });
        }

        [HttpGet("get-images")]
        public IActionResult GetPropertyImages([FromQuery(Name = "chemical_id")] int chemicalId, [FromQuery(Name = "property_id")] int propertyId)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.connectionString);
            var images = db.Query<ImageModel>(
            @"SELECT [Название файла графика] as ImagePath, [Подпись к рисунку] as Name FROM dbo.[Ссылки на графики] 
                WHERE [Номер соединения] = @chemicalId AND [Номер свойства] = @propertyId"
            , new { chemicalId, propertyId }).ToList();

            if (images.Count == 0)
            {
                return BadRequest("No charts");
            }
            ImageQueryModel imageQuery = new();
            imageQuery.ChemicalFormula = GetChemicalFormula(chemicalId);
            imageQuery.PropertyName = GetPropertyNameRus(propertyId);
            imageQuery.Images = images;
            return View("_ImagesPartialView", imageQuery);
        }
    }
}
