using Coursework_CrystalSite_Final.Models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Coursework_CrystalSite_Final.Controllers
{
    /// <summary>
    /// Контроллер для обработки различных запросов по литературе.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("books")]
    public class BookController : Controller
    {
        /// <summary>
        /// Возвращает представление с литературой (книга/статья) по ее номеру.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("id/{id}")]
        public IActionResult GetBookById([FromRoute] int id)
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.ConnectionString);
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

        /// <summary>
        /// Возвращает основное представление для поиска по книгам.
        /// </summary>
        /// <returns></returns>
        [HttpGet("search-setup")]
        [AllowAnonymous]
        public IActionResult SearchView()
        {
            return View("SearchView");
        }

        /// <summary>
        /// Обрабатывает запрос на поиск по книгам.
        /// </summary>
        /// <returns></returns>
        [HttpGet("search-result")]
        public IActionResult SearchResult()
        {
            using IDbConnection db = new SqlConnection(DatabaseConnection.ConnectionString);
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
