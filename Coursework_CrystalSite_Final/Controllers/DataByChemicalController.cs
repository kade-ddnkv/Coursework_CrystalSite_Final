using Coursework_CrystalSite_Final.Models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Coursework_CrystalSite_Final.Controllers
{
    /// <summary>
    /// Контроллер для обработки запросов по свойствам соединений.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("data-by-chemical")]
    public class DataByChemicalController : Controller
    {
        /// <summary>
        /// Отображение сокращенных имен сингоний, используемых в БД, в полные названия.
        /// </summary>
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

        /// <summary>
        /// Возвращает представление со свойствами, доступными для текущего соединения.
        /// </summary>
        /// <param name="chemicalUrlName"></param>
        /// <returns></returns>
        [HttpGet("ch/{chemicalUrlName}/")]
        public IActionResult GetChemicalProperties([FromRoute] string chemicalUrlName)
        {
            if (!ChemicalModel.UrlNameToId.ContainsKey(chemicalUrlName))
            {
                return NotFound("No chemical with such name");
            }
            int chemicalId = ChemicalModel.UrlNameToId[chemicalUrlName];
            using IDbConnection db = new SqlConnection(DatabaseConnection.ConnectionString);
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

        /// <summary>
        /// Возвращает результат запроса в БД по номеру свойства и номеру соединения.
        /// Запрос делается именно в таблицах БД (с английскими названиями).
        /// </summary>
        /// <param name="chemicalId"></param>
        /// <param name="propertyId"></param>
        /// <returns></returns>
        [NonAction]
        public (string, IEnumerable<dynamic>) GetPropertyValuesFromInnerTables(int chemicalId, int propertyId)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.ConnectionString);
            string nameOfPropertyEn = db.QuerySingleOrDefault<dynamic>(
                "SELECT TableName AS TableNameEn FROM dbo.Properties WHERE NOMPROP = @propertyId"
                , new { propertyId }).TableNameEn;
            return (nameOfPropertyEn, db.Query<dynamic>(
                $"SELECT * FROM dbo.[{nameOfPropertyEn}] WHERE HeadClue = @chemicalId"
                , new { chemicalId }));
        }

        /// <summary>
        /// Исправляет имя свойства на название соответствующего ему представления в БД.
        /// </summary>
        /// <param name="nameOfPropertyRus"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Возвращает результат запроса в БД по ррусскому названию свойства и номеру соединения.
        /// Запрос делается в представлениях БД (с русскими названиями).
        /// </summary>
        /// <param name="chemicalId"></param>
        /// <param name="propertyNameRus"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> GetPropertyValuesFromViews(int chemicalId, string propertyNameRus)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.ConnectionString);
            return db.Query<dynamic>(
                $"SELECT * FROM dbo.[{propertyNameRus}] WHERE [Номер соединения] = @chemicalId"
                , new { chemicalId });
        }

        /// <summary>
        /// Возвращает русское название свойства по его номеру.
        /// </summary>
        /// <param name="propertyId"></param>
        /// <returns></returns>
        [NonAction]
        public string GetPropertyViewNameRus(int propertyId)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.ConnectionString);
            string nameOfPropertyRus = db.QuerySingleOrDefault<dynamic>(
                "SELECT NAZVPROP as ViewNameRus FROM dbo.Properties WHERE NOMPROP = @propertyId"
                , new { propertyId }).ViewNameRus;
            nameOfPropertyRus = FixNameOfPropertyToView(nameOfPropertyRus);
            return nameOfPropertyRus;
        }

        /// <summary>
        /// Обработка нестандартного свойства ("Аналитический обзор").
        /// </summary>
        /// <param name="chemicalId"></param>
        /// <returns></returns>
        [NonAction]
        public (string ChemicalName, string AnalyticalReview) GetAnalyticalReview(int chemicalId)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.ConnectionString);
            string analyticalReview = db.QueryFirstOrDefault<dynamic>(
            $"SELECT [Аналитический обзор] as AnalyticalReview FROM dbo.[Аналитический обзор] WHERE [Номер соединения] = @chemicalId"
            , new { chemicalId }).AnalyticalReview;
            analyticalReview = analyticalReview.Replace("bk.asp?Bknumber=", "/books/id/");
            analyticalReview = analyticalReview.Replace("/ru/Pictures/pdf.gif", "https://crystal.imet-db.ru/ru/Pictures/pdf.gif");
            return (ChemicalModel.IdToHtmlName[chemicalId], analyticalReview);
        }

        /// <summary>
        /// Возвращает формулу соединения по его номеру.
        /// </summary>
        /// <param name="chemicalId"></param>
        /// <returns></returns>
        [HttpGet("get-chemical-formula")]
        public string GetChemicalFormula([FromQuery(Name = "chemical_id")] int chemicalId)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.ConnectionString);
            return db.QueryFirstOrDefault<dynamic>(
            @"SELECT HeadClue as Id, System as HtmlName FROM dbo._HeadTablConv WHERE HeadClue = @chemicalId"
            , new { chemicalId }).HtmlName;
        }

        /// <summary>
        /// Превращает результат запроса в БД в модель для представления (View).
        /// </summary>
        /// <param name="chemicalHtmlName"></param>
        /// <param name="propertyNameRus"></param>
        /// <param name="queryResult"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Вспомогательный метод для нестандартного свойства "Нелинейные оптические свойства".
        /// </summary>
        /// <param name="chemicalId"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        [NonAction]
        public dynamic TempForNonLinear(int chemicalId, string propertyName)
        {
            var queryResult = GetPropertyValuesFromViews(chemicalId, propertyName);
            return MakeTableModel(ChemicalModel.IdToHtmlName[chemicalId], propertyName, queryResult);
        }

        /// <summary>
        /// Обработка нестандартного свойства ("Нелинейные оптические свойства").
        /// </summary>
        /// <param name="chemicalId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Обработка нестандартного свойства ("Литература").
        /// </summary>
        /// <param name="chemicalId"></param>
        /// <returns></returns>
        [NonAction]
        public (string ChemicalName, IEnumerable<BookModel> Books) GetLiterature(int chemicalId)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.ConnectionString);
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

        /// <summary>
        /// Основной управляющий метод для запроса данных о свойстве вещества.
        /// Возвращает представление с результатами запроса в БД, представленными в читабельном виде.
        /// </summary>
        /// <param name="chemicalUrlName"></param>
        /// <param name="propertyUrlName"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
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

        /// <summary>
        /// Возвращает список графиков (моделей графиков) по номеру соединения и номеру свойства.
        /// </summary>
        /// <param name="chemicalId"></param>
        /// <param name="propertyId"></param>
        /// <returns></returns>
        [NonAction]
        public List<ImageModel> GetImages(int chemicalId, int propertyId)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.ConnectionString);
            return db.Query<ImageModel>(
            @"SELECT [Название файла графика] as ImagePath, [Подпись к рисунку] as Name FROM dbo.[Ссылки на графики] 
                WHERE [Номер соединения] = @chemicalId AND [Номер свойства] = @propertyId"
            , new { chemicalId, propertyId }).ToList();
        }
    }
}
