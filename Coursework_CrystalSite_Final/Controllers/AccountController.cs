using Coursework_CrystalSite_Final.Models;
using Dapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;

namespace Coursework_CrystalSite_Final.Controllers
{
    /// <summary>
    /// Контроллер, отвечающий за аутентификацию, авторизацию, регистрацию.
    /// </summary>
    [Route("account")]
    public class AccountController : Controller
    {
        /// <summary>
        /// Список с подстроками User-Agent, которые могут указывать на то, что запрос исходит от поискового бота.
        /// </summary>
        private List<string> spiderBots = new()
        {
            "googlebot",
            "google.com/bot.html",

            "bingbot",
            "bing.com/bingbot.htm",

            "slurp",
            "help.yahoo.com/help/us/ysearch/slurp",

            "duckduckbot",
            "duckduckgo.com/duckduckbot.html",

            "baiduspider",
            "baidu.com/search/spider.html",

            "yandexbot",
            "yandex.com/bots",

            "sogou",
            "sogou.com/docs/help/webmasters.htm#07",

            "exabot",
            "exabot.com/go/robot",

            "facebot",
            "facebookexternalhit",

            "ia_archiver",
            "alexa.com/site/help/webmasters",
        };

        /// <summary>
        /// Определяет по User-Agent, является ли запрос к Сервису запросом от поискового бота (для индексации).
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        [NonAction]
        private bool IsSearchBot(string userAgent)
        {
            // Для примера: мой браузер.
            // return userAgent == @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/101.0.4951.41 Safari/537.36 Edg/101.0.1210.32";

            string lowerUserAgent = userAgent.ToLower();
            foreach (string botString in spiderBots)
            {
                if (lowerUserAgent.Contains(botString))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Возвращает View для логина, если запрос не исходит от поискового бота.
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet("signin")]
        public async Task<IActionResult> SignInGetAsync(string? returnUrl)
        {
            string userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            // Поисковых ботов считать авторизованными.
            if (IsSearchBot(userAgent))
            {
                var claimsBot = new List<Claim> { new Claim(ClaimTypes.Name, "SearchBot") };
                ClaimsIdentity claimsBotIdentity = new ClaimsIdentity(claimsBot, "Cookies");
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsBotIdentity));
                return Redirect(returnUrl ?? "/");
            }
            return View("SignIn", returnUrl != null);
        }

        /// <summary>
        /// Обрабатывает запрос на аутентификацию от пользователя.
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost("signin")]
        public async Task<IActionResult> SignInPostAsync(string? returnUrl)
        {
            var form = HttpContext.Request.Form;
            string email = form["email"];
            string password = form["password"];

            // Аутентификация через локальную базу данных.

            //using IDbConnection db = new SqlConnection(DatabaseConnection.connectionString);
            //Account? person = db.QuerySingleOrDefault<Account>(
            //    @"SELECT * FROM dbo.[Users] WHERE Email = @email AND Password = @password",
            //    new { email, password });
            //if (person is null)
            //{
            //    return View("NoSuchUser");
            //}

            // Аутентификация через запрос на SSO.IMET-DB.

            string ssoRoot = "https://sso.imet-db.ru/sso/";
            string host = "crystal.imet-db.ru";
            string port = "443";
            int randNumber = (int)(new Random().NextDouble() * 100000);

            string queryParams = "mode=login&host=" + host + "&port=" + port + "&login=" + email + "&pass=" + password + "&persist=" + 0 +"&r=" + randNumber;

            var content = new StringContent("", System.Text.Encoding.UTF8, "application/json");
            HttpClient client = new();
            HttpResponseMessage response = await client.PostAsync(ssoRoot + "json_auth.ashx?" + queryParams, content);
            response.EnsureSuccessStatusCode();
            dynamic responseJson = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
            if (responseJson.Msg != "")
            {
                return StatusCode(500);
            }
            if (responseJson.Data == null)
            {
                return View("NoSuchUser");
            }

            // Запись аутентификации через Claims в куки.
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, email) };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            return Redirect(returnUrl ?? "/");
        }

        /// <summary>
        /// Обрабатывает запрос от пользователя на выход из учетной записи.
        /// </summary>
        /// <returns></returns>
        [HttpGet("signout")]
        public async Task<IActionResult> SignOutAsync()
        {
            string userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            if (IsSearchBot(userAgent))
            {
                return Redirect("/");
            }
            
            // Локальное разлогинивание.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Разлогинивание в SSO.IMET-DB
            string ssoRoot = "https://sso.imet-db.ru/sso/";
            string host = "crystal.imet-db.ru";
            string port = "443";
            int randNumber = (int)(new Random().NextDouble() * 100000);
            var queryParams = "mode=logoff&host=" + host + "&port=" + port + "&r=" + randNumber;
            var content = new StringContent("", System.Text.Encoding.UTF8, "application/json");
            HttpClient client = new();
            HttpResponseMessage response = await client.PostAsync(ssoRoot + "json_auth.ashx?" + queryParams, content);
            response.EnsureSuccessStatusCode();
            dynamic responseJson = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
            if (responseJson.Msg != "")
            {
                return StatusCode(500);
            }

            return Redirect("/");
        }
    }
}
