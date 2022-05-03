using Coursework_CrystalSite_Final.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Coursework_CrystalSite_Final.Controllers
{
    [ApiController]
    [Route("search-chemicals")]
    public class SearchChemicalsController : Controller
    {
        [HttpGet("index")]
        public IActionResult Index()
        {
            return View("Index");
        }

        /// <summary>
        /// Метод, возвращающий соединения по находящимся в них элементам.
        /// Для фильтрации он использует функцию Transact Sql LIKE, 
        /// которая обладает весьма ограниченным функционалом.
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        [HttpGet("by-elements")]
        public IActionResult GetChemicalsByElements([FromQuery] string elements)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.connectionString);
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
    }
}
