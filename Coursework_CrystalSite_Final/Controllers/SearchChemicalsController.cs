using Coursework_CrystalSite_Final.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Coursework_CrystalSite_Final.Controllers
{
    /// <summary>
    /// Контроллер, отвечающий за поиск соединений через таблицу Менделеева.
    /// </summary>
    [ApiController]
    [Route("search-chemicals")]
    public class SearchChemicalsController : Controller
    {
        /// <summary>
        /// Возвращает основное представление для поиска соединений по таблице Менделеева.
        /// </summary>
        /// <returns></returns>
        [HttpGet("index")]
        public IActionResult Index()
        {
            return View("Index");
        }

        /// <summary>
        /// Метод, возвращающий соединения по находящимся в них элементам.
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        [HttpGet("by-elements")]
        public IActionResult GetChemicalsByElements([FromQuery] string elements)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.ConnectionString);
            string[] chemStrings = elements.Split('-', StringSplitOptions.RemoveEmptyEntries);
            List<ChemicalModel> chemicals = new();
            if (chemStrings.Length != 0)
            {
                string query = @"SELECT HeadClue as Id, System as HtmlName FROM dbo._HeadTablConv ";
                string filter = "WHERE " + string.Join(" AND ", chemStrings.Select(chem => $"Help LIKE '%-{chem}-%'"));
                chemicals = db.Query<ChemicalModel>(query + filter).ToList();
            }
            chemicals.Sort(ChemicalModel.CompareChemicals);
            return PartialView("_FoundChemicalsPartialView", chemicals);
        }

        /// <summary>
        /// Возвращает представление со всеми доступными соединениями в БД.
        /// (Для хорошей индексации сайта)
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        public IActionResult AllChemicals()
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.ConnectionString);
            List<ChemicalModel> chemicals = db.Query<ChemicalModel>(
                @"SELECT HeadClue as Id, System as HtmlName FROM dbo._HeadTablConv").ToList();
            chemicals.Sort(ChemicalModel.CompareChemicals);
            return View("AllChemicals", chemicals);
        }
    }
}
