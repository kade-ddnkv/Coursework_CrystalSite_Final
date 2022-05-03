using Coursework_CrystalSite_Final.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Coursework_CrystalSite_Final.Controllers
{
    [ApiController]
    [Route("books")]
    public class BookController : Controller
    {
        [HttpGet("id/{id}")]
        public IActionResult GetBookById([FromRoute] int id)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.connectionString);
            BookModel book = db.QueryFirst<BookModel>(
                @"SELECT [Номер ссылки] as BookNumber
	                , [Ф.И.О. авторов] as Authors
	                , [Выходные данные] as BookNameAndPages
	                , [Название статьи/монографии] as ArticleName
	                , DOI as DoiLink
                FROM dbo.[Библиографические ссылки]
                WHERE [Номер ссылки] = @bookId"
                , new { bookId = id });
            return View("OneBook", book);
        }

        [HttpGet("search-setup")]
        public IActionResult SearchView()
        {
            return View("SearchView");
        }

        [HttpGet("search-result")]
        public IActionResult SearchResult()
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.connectionString);
            string query = @"SELECT [Номер ссылки] as BookNumber
	                            , [Ф.И.О. авторов] as Authors
	                            , [Выходные данные] as BookNameAndPages
	                            , [Название статьи/монографии] as ArticleName
	                            , DOI as DoiLink
                            FROM dbo.[Библиографические ссылки] ";
            string filter = "";
            List<string> authors = new();
            foreach (string key in Request.Query.Keys)
            {
                authors.Add(Request.Query[key]);
            }
            if (authors.Count > 0)
            {
                filter = "WHERE " + string.Join(" AND ", authors.Select(author => $"[Ф.И.О. авторов] LIKE '%{author}%'"));
            }
            IEnumerable<BookModel> books = db.Query<BookModel>(query + filter);
            return View("SearchResult", books);
        }
    }
}
