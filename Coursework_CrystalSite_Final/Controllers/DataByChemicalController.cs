using Coursework_CrystalSite_Final.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Coursework_CrystalSite_Final.Controllers
{
    [ApiController]
    [Route("data-by-chemical")]
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

        [HttpGet("ch/{chemicalUrlName}/")]
        public IActionResult GetChemicalProperties([FromRoute] string chemicalUrlName)
        {
            if (!ChemicalModel.UrlNameToId.ContainsKey(chemicalUrlName))
            {
                return NotFound("No chemical with such name");
            }
            int chemicalId = ChemicalModel.UrlNameToId[chemicalUrlName];
            using IDbConnection db = new SqlConnection(DatabaseConnection.connectionString);
            IEnumerable<PropertyModel> properties = db.Query<PropertyModel>(@"
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
            return View("Properties", (chemicalUrlName, properties));
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

        public IEnumerable<dynamic> GetPropertyValuesFromViews(int chemicalId, string propertyNameRus)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.connectionString);
            return db.Query<dynamic>(
                $"SELECT * FROM dbo.[{propertyNameRus}] WHERE [Номер соединения] = @chemicalId"
                , new { chemicalId });
        }

        [NonAction]
        public string GetPropertyViewNameRus(int propertyId)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.connectionString);
            string nameOfPropertyRus = db.QuerySingleOrDefault<dynamic>(
                "SELECT NAZVPROP as ViewNameRus FROM dbo.Properties WHERE NOMPROP = @propertyId"
                , new { propertyId }).ViewNameRus;
            nameOfPropertyRus = FixNameOfPropertyToView(nameOfPropertyRus);
            return nameOfPropertyRus;
        }

        [NonAction]
        public (string ChemicalName, string AnalyticalReview) GetAnalyticalReview(int chemicalId)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.connectionString);
            string analyticalReview = db.QueryFirstOrDefault<dynamic>(
            $"SELECT [Аналитический обзор] as AnalyticalReview FROM dbo.[Аналитический обзор] WHERE [Номер соединения] = @chemicalId"
            , new { chemicalId }).AnalyticalReview;
            analyticalReview = analyticalReview.Replace("bk.asp?Bknumber=", "/books/id/");
            analyticalReview = analyticalReview.Replace("/ru/Pictures/pdf.gif", "https://crystal.imet-db.ru/ru/Pictures/pdf.gif");
            return (ChemicalModel.IdToHtmlName[chemicalId], analyticalReview);
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
        public PropertyTableModel MakeTableModel(string chemicalHtmlName, string propertyNameRus, IEnumerable<dynamic> queryResult)
        {
            PropertyTableModel tableModel = new()
            {
                PropertyName = propertyNameRus,
                ChemicalName = chemicalHtmlName,
            };
            // Если ответ пустой, то поля tableModel останутся null.
            if (queryResult.Count() != 0)
            {
                // Разделяю на заголовки и строки.
                List<string> columnNames = ((IDictionary<string, object>)queryResult.FirstOrDefault()).Keys.Skip(1).ToList();
                // В строках пропускаю первый столбец - это номер элемента.
                // Последний столбец - это ссылка на литературу.
                IEnumerable<List<object>> rows = queryResult.Select(row =>
                    {
                        var rowOfValues = ((IDictionary<string, dynamic>)row).Values.Skip(1).ToList();
                        rowOfValues[rowOfValues.Count - 1] = LinkBuilder.CreateBookLink(rowOfValues.Last());
                        return rowOfValues;
                    });

                if (columnNames.Contains("Сингония"))
                {
                    int syngColumnIndex = columnNames.IndexOf("Сингония");
                    List<SyngonyModel> syngGroups = rows
                        .GroupBy(row => row[syngColumnIndex])
                        .Select(group => new SyngonyModel()
                        {
                            Name = syngonyNamesMapper[group.Key as string],
                            Rows = group.Select(row => row.Skip(1))
                        })
                        .ToList();
                    tableModel.TableWithSyngonyModel = new()
                    {
                        ColumnNames = columnNames.Skip(1).ToList(),
                        SyngGroups = syngGroups
                    };
                }
                else
                {
                    tableModel.TableWithoutSyngonyModel = new()
                    {
                        ColumnNames = columnNames,
                        Rows = rows,
                    };
                }
            }
            return tableModel;
        }

        [NonAction]
        public dynamic TempForNonLinear(int chemicalId, string propertyName)
        {
            var queryResult = GetPropertyValuesFromViews(chemicalId, propertyName);
            return MakeTableModel(ChemicalModel.IdToHtmlName[chemicalId], propertyName, queryResult);
        }

        [NonAction]
        public (string ChemicalName, PropertyTableModel NonLinearOpticalCoefficients, 
            PropertyTableModel ComponentsOfTheMillerTensor, List<ImageModel> Images)
            GetNonLinearOpticalPropertiesValues(int chemicalId)
        {
            var nonLinearOpticalCoefficients = TempForNonLinear(chemicalId, "Нелинейные оптические коэффициенты");
            var componentsOfTheMillerTensor = TempForNonLinear(chemicalId, "Компоненты тензора Миллера (10-2 м/Кл)");
            return (ChemicalModel.IdToHtmlName[chemicalId], nonLinearOpticalCoefficients, 
                componentsOfTheMillerTensor, GetImages(chemicalId, 29));
        }

        [NonAction]
        public (string ChemicalName, IEnumerable<BookModel> Books) GetLiterature(int chemicalId)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.connectionString);
            return (ChemicalModel.IdToHtmlName[chemicalId], db.Query<BookModel>(
                @"SELECT litr.[Номер ссылки] as BookNumber
	                , [Ф.И.О. авторов] as Authors
	                , [Выходные данные] as BookNameAndPages
	                , [Название статьи/монографии] as ArticleName
	                , DOI as DoiLink
                FROM dbo.[Литературные ссылки] as litr
                JOIN dbo.[Библиографические ссылки] as bibl ON litr.[Номер ссылки] = bibl.[Номер ссылки]
                WHERE [Номер соединения] = @chemicalId"
                , new { chemicalId }));
        }

        [HttpGet("ch/{chemicalUrlName}/{propertyUrlName}/")]
        public IActionResult GetPropertyValues([FromRoute] string chemicalUrlName, [FromRoute] string propertyUrlName)
        {
            if (!ChemicalModel.UrlNameToId.ContainsKey(chemicalUrlName))
            {
                return NotFound("No chemical with such name");
            }
            if (!PropertyModel.UrlNameToId.ContainsKey(propertyUrlName))
            {
                return NotFound("No property with such name");
            }
            int chemicalId = ChemicalModel.UrlNameToId[chemicalUrlName];
            int propertyId = PropertyModel.UrlNameToId[propertyUrlName];
            // Особые свойства обрабатываются отдельно.
            switch (propertyId)
            {
                case 1:
                    // "Аналитический обзор"
                    return View("AnalyticalReview", GetAnalyticalReview(chemicalId));
                case 4:
                    // "Состав соединения"
                    return View("ChemicalFormula", ChemicalModel.IdToHtmlName[chemicalId]);
                case 25:
                    // "Нелинейные оптические свойства"
                    return View("NonLinearOpticalProperties", GetNonLinearOpticalPropertiesValues(chemicalId));
                case 29:
                    // "Литература"
                    return View("Literature", GetLiterature(chemicalId));
            }

            string propertyNameRus;
            IEnumerable<dynamic> queryResult;
            try
            {
                propertyNameRus = GetPropertyViewNameRus(propertyId);
                queryResult = GetPropertyValuesFromViews(chemicalId, propertyNameRus);
            }
            catch (SqlException)
            {
                throw new NotImplementedException("Special property that doesn't have a russian view. Rememeber it and hadle this property in a different way.");
                (propertyNameRus, queryResult) = GetPropertyValuesFromInnerTables(chemicalId, propertyId);
            }

            return View("PropertyTable",
                (MakeTableModel(ChemicalModel.IdToHtmlName[chemicalId], propertyNameRus, queryResult), GetImages(chemicalId, propertyId)));
        }

        [NonAction]
        public List<ImageModel> GetImages(int chemicalId, int propertyId)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.connectionString);
            return db.Query<ImageModel>(
            @"SELECT [Название файла графика] as ImagePath, [Подпись к рисунку] as Name FROM dbo.[Ссылки на графики] 
                WHERE [Номер соединения] = @chemicalId AND [Номер свойства] = @propertyId"
            , new { chemicalId, propertyId }).ToList();
        }
    }
}
