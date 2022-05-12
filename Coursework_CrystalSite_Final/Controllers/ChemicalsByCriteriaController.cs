//using Dapper;
using Coursework_CrystalSite_Final.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Coursework_CrystalSite_Final.Controllers
{
    /// <summary>
    /// Контроллер для обработки многокритериальных запросов для поиска соединений.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("chemicals-by-criteria")]
    public class ChemicalsByCriteriaController : Controller
    {
        /// <summary>
        /// Возвращает основное представление для настройки многокритериального поиска.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("query-setup")]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Отображает русское название свойства в название соответствующего представления в БД.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        [NonAction]
        private string PropertyNameToViewName(string propertyName)
        {
            switch (propertyName)
            {
                case "Сингония":
                    return "Сингонии соединений";
                case "Точечная группа":
                    return "Характеристика кристаллической структуры";
                case "Компоненты тензора Миллера":
                    return "Компоненты тензора Миллера (10-2 м/Кл)";
                case "Коэффициент затухания (D) или скорость распространения (S) упругих волн":
                    return "Распространение и затухание упругих волн";
                default:
                    return propertyName;
            }
        }

        /// <summary>
        /// Определяет, делается ли многокритериальный запрос только по одному критерию.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        [NonAction]
        private bool IsOnlyOneCriteria(Dictionary<string, Dictionary<string, string>> properties)
        {
            return properties.Count == 1 && properties.Keys.First() != "Состав соединения";
        }

        /// <summary>
        /// Кодирует греческую букву в ее html-код.
        /// </summary>
        /// <param name="letter"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        [NonAction]
        private string GreekLettersEncode(string letter)
        {
            switch (letter)
            {
                case "α":
                    return "&alpha;";
                case "β":
                    return "&beta;";
                case "γ":
                    return "&gamma;";
                default:
                    throw new NotSupportedException("This greek letter is not supported");
            }
        }

        /// <summary>
        /// Основной метод для многокритериального запроса.
        /// Строит запрос в БД по критериям.
        /// Возвращает представление с результатами запроса.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        [NonAction]
        private IActionResult MultipleCriteriaSearch(Dictionary<string, Dictionary<string, string>> properties)
        {
            var connection = new SqlConnection(DatabaseConnection.ConnectionString);
            var compiler = new SqlServerCompiler();
            var db = new QueryFactory(connection, compiler);

            var kataQuery = db.Query("Соединения").Select($"Соединения.Соединение").Distinct();
            string viewName;
            foreach (var prop in properties)
            {
                switch (prop.Key)
                {
                    case "Состав соединения":
                        kataQuery.Join($"_HeadTablConv", $"Соединения.Номер соединения", $"_HeadTablConv.HeadClue");
                        foreach (string inputName in prop.Value.Keys)
                        {
                            if (inputName.Contains("el"))
                            {
                                kataQuery.WhereLike($"_HeadTablConv.Help", $"%-{prop.Value[inputName]}-%");
                            }
                        }
                        break;

                    case "Температура плавления":
                        kataQuery.Join($"{prop.Key}", $"Соединения.Номер соединения", $"{prop.Key}.Номер соединения")
                            .WhereBetween($"{prop.Key}.Температура, K", $"{prop.Value["температура левая"]}", $"{prop.Value["температура правая"]}")
                            .Select($"{prop.Key}.Тип плавления", $"{prop.Key}.Температура, K");
                        if (prop.Value["тип"] != "any")
                        {
                            kataQuery.Where($"{prop.Key}.Тип плавления", $"{prop.Value["тип"]}");
                        }
                        break;

                    case "Плотность":
                        kataQuery.Join($"{prop.Key}", $"Соединения.Номер соединения", $"{prop.Key}.Номер соединения")
                            .WhereBetween($"{prop.Key}.Плотность, г/см3", $"{prop.Value["плотность левая"]}", $"{prop.Value["плотность правая"]}")
                            .Select($"{prop.Key}.Плотность, г/см3");
                        break;

                    case "Твердость":
                        if (prop.Value["тип"] == "Моос")
                        {
                            kataQuery.Join($"{prop.Key}", $"Соединения.Номер соединения", $"{prop.Key}.Номер соединения")
                                .WhereBetween($"{prop.Key}.Моос", $"{prop.Value["твердость левая"]}", $"{prop.Value["твердость правая"]}")
                                .Select($"{prop.Key}.Моос");
                        }
                        else if (prop.Value["тип"] == "ГПа")
                        {
                            kataQuery.Join($"{prop.Key}", $"Соединения.Номер соединения", $"{prop.Key}.Номер соединения")
                                .WhereRaw($"{prop.Key}.Min >= {prop.Value["твердость левая"]}")
                                .WhereRaw($"{prop.Key}.Max <= {prop.Value["твердость правая"]}")
                                .Select($"{prop.Key}.Min as Твердость Min", $"{prop.Key}.Max as Твердость Max");
                        }
                        break;

                    case "Удельная теплоемкость":
                        kataQuery.Join($"{prop.Key}", $"Соединения.Номер соединения", $"{prop.Key}.Номер соединения")
                            .WhereBetween($"{prop.Key}.Температура, K", $"{prop.Value["температура левая"]}", $"{prop.Value["температура правая"]}")
                            .WhereBetween($"{prop.Key}.Теплоемко-сть, Дж/кг*K", $"{prop.Value["теплоемкость левая"]}", $"{prop.Value["теплоемкость правая"]}")
                            .Select($"{prop.Key}.Обозначение коэффициента", $"{prop.Key}.Температура, K", $"{prop.Key}.Теплоемко-сть, Дж/кг*K");
                        if (prop.Value["тип"] != "any")
                        {
                            kataQuery.Where($"{prop.Key}.Обозначение коэффициента", $"{prop.Value["тип"]}");
                        }
                        break;

                    case "Растворимость":
                        kataQuery.Join($"{prop.Key}", $"Соединения.Номер соединения", $"{prop.Key}.Номер соединения")
                            .WhereBetween($"{prop.Key}.Температура, K", $"{prop.Value["температура левая"]}", $"{prop.Value["температура правая"]}")
                            .WhereBetween($"{prop.Key}.Растворимость, г/100 г раствора", $"{prop.Value["растворимость левая"]}", $"{prop.Value["растворимость правая"]}")
                            .Select($"{prop.Key}.Растворитель", $"{prop.Key}.Температура, K", $"{prop.Key}.Растворимость, г/100 г раствора");
                        kataQuery.Where($"{prop.Key}.Растворитель", $"{prop.Value["растворитель"]}");
                        break;

                    case "Температура Кюри":
                        kataQuery.Join($"{prop.Key}", $"Соединения.Номер соединения", $"{prop.Key}.Номер соединения")
                            .WhereBetween($"{prop.Key}.Температура, K", $"{prop.Value["температура левая"]}", $"{prop.Value["температура правая"]}")
                            .Select($"{prop.Key}.Температура, K");
                        break;

                    case "Сингония":
                        viewName = PropertyNameToViewName(prop.Key);
                        kataQuery.Join($"{viewName}", $"Соединения.Номер соединения", $"{viewName}.Номер соединения")
                            .Where($"{viewName}.Обозначение сингонии", $"{prop.Value["сингония"]}")
                            .Select($"{viewName}.Обозначение сингонии");
                        break;

                    case "Точечная группа":
                        viewName = PropertyNameToViewName(prop.Key);
                        kataQuery.Join($"{viewName}", $"Соединения.Номер соединения", $"{viewName}.Номер соединения")
                            .Where($"{viewName}.Точечная группа симметрии", $"{prop.Value["тип"]}")
                            .Select($"{viewName}.Точечная группа симметрии");
                        break;

                    case "Параметры элементарной ячейки":
                        kataQuery.Join($"{prop.Key}", $"Соединения.Номер соединения", $"{prop.Key}.Номер соединения");
                        HashSet<string> activeA = new();
                        HashSet<string> activeAngles = new();
                        foreach (string inputName in prop.Value.Keys)
                        {

                            string type = inputName.Split(" ")[0];
                            if (type == "a" || type == "b" || type == "c")
                            {
                                activeA.Add(type);
                            }
                            else
                            {
                                activeAngles.Add(type);
                            }
                        }
                        if (activeA.Count > 0)
                        {
                            string whereActiveA = string.Join(" OR ", activeA.Select(a =>
                            {
                                string left = prop.Value.ContainsKey($"{a} левая") ? $"AND [Значение линейного параметра, A] >= {prop.Value[$"{a} левая"]}" : "";
                                string right = prop.Value.ContainsKey($"{a} правая") ? $"AND [Значение линейного параметра, A] <= {prop.Value[$"{a} правая"]}" : "";
                                string orWhere = $"([Название линейного параметра] = '{a}' {left} {right})";
                                return orWhere;
                            }));
                            whereActiveA += $" OR ({string.Join(" AND ", activeA.Select(a => $"[Название линейного параметра] != '{a}'"))})";
                            kataQuery.WhereRaw("(" + whereActiveA + ")");
                            kataQuery.Select($"Название линейного параметра as Парам. решетки (назв.)", $"Значение линейного параметра, A as Парам. решетки (знач.)");
                        }
                        if (activeAngles.Count > 0)
                        {
                            string whereActiveAngles = string.Join(" OR ", activeAngles.Select(angle =>
                            {
                                string left = prop.Value.ContainsKey($"{angle} левая") ? $"AND [Значение угла, град.] >= {prop.Value[$"{angle} левая"]}" : "";
                                string right = prop.Value.ContainsKey($"{angle} правая") ? $"AND [Значение угла, град.] <= {prop.Value[$"{angle} правая"]}" : "";
                                string orWhere = $"([Название угла] = '{GreekLettersEncode(angle)}' {left} {right})";
                                return orWhere;
                            }));
                            whereActiveAngles += $" OR ({string.Join(" AND ", activeAngles.Select(angle => $"[Название угла] != '{GreekLettersEncode(angle)}'"))})";
                            whereActiveAngles += $" OR ([Название угла] IS NULL)";
                            kataQuery.WhereRaw("(" + whereActiveAngles + ")");
                            kataQuery.Select($"Название угла as Парам. элем. ячейки (назв. угла)", $"Значение угла, град. as Парам. элем. ячейки (знач. угла)");
                        }
                        break;

                    case "Тепловое расширение":
                        JoinOneRangeCriteria(kataQuery, prop, "Значение коэффициента", "расширение",
                            ("Обозначение коэффициента", "Тепл. расшир. (обозн. коэф.)"), ("Значение коэффициента", "Тепл. расшир. (знач. коэф.)"));
                        break;

                    case "Теплопроводность":
                        JoinOneRangeCriteria(kataQuery, prop, "Значение коэффициента", "теплопроводность",
                            ("Обозначение коэффициента", "Теплопр-ть (обозн. коэф.)"), ("Значение коэффициента", "Теплопр-ть (знач. коэф.)"));
                        break;

                    case "Диэлектрическая проницаемость":
                        JoinOneRangeCriteria(kataQuery, prop, "Значение коэффициента", "проницаемость",
                            ("Обозначение коэффициента", "Диэл. прониц. (обозн. коэф.)"), ("Значение коэффициента", "Диэл. прониц. (знач. коэф.)"));
                        break;

                    case "Тангенс угла диэлектрических потерь":
                        JoinOneRangeCriteria(kataQuery, prop, "Значение тангенса угла", "тангенс",
                            ("Обозначение тангенса угла потерь", "Обозначение тангенса угла потерь"), ("Значение тангенса угла", "Значение тангенса угла"));
                        break;

                    case "Пьезоэлектрические коэффициенты":
                        viewName = PropertyNameToViewName(prop.Key);
                        kataQuery.Join($"{viewName}", $"Соединения.Номер соединения", $"{viewName}.Номер соединения")
                            .WhereLike($"{viewName}.Обозначение коэффициента", $"%{prop.Value["параметр"]}%")
                            .WhereBetween($"{prop.Key}.Значение коэффициента"
                                , $"{prop.Value[prop.Value["параметр"] + " левая"]}"
                                , $"{prop.Value[prop.Value["параметр"] + " правая"]}")
                            .Select($"{viewName}.Обозначение коэффициента as Пьезоэл. (обозн. коэф.)")
                            .Select($"{viewName}.Значение коэффициента as Пьезоэл. (знач. коэф.)");
                        break;

                    case "Коэффициенты электромеханической связи":
                        JoinOneRangeCriteria(kataQuery, prop, "Значение коэффицента", "k",
                            ("Обозначение коэффициента", "Эл.-механ. связь (обозн. коэф.)"), ("Значение коэффицента", "Эл.-механ. связь (знач. коэф.)"));
                        break;

                    case "Упругие постоянные":
                        viewName = PropertyNameToViewName(prop.Key);
                        kataQuery.Join($"{viewName}", $"Соединения.Номер соединения", $"{viewName}.Номер соединения")
                            .WhereLike($"{viewName}.Условия измерения", $"%{prop.Value["параметр"]}%")
                            .WhereBetween($"{prop.Key}.Обозначение коэффициента"
                                , $"{prop.Value[prop.Value["параметр"] + " левая"]}"
                                , $"{prop.Value[prop.Value["параметр"] + " правая"]}")
                            .Select($"{viewName}.Условия измерения as Упруг. пост. (параметр)")
                            .Select($"{viewName}.Обозначение коэффициента as Упруг. пост. (знач. коэф.)");
                        break;

                    case "Полоса пропускания":
                        viewName = PropertyNameToViewName(prop.Key);
                        kataQuery.Join($"{viewName}", $"Соединения.Номер соединения", $"{viewName}.Номер соединения")
                            .WhereBetween($"{prop.Key}.Нижняя граница", $"{prop.Value["B левая"]}", $"{prop.Value["B правая"]}")
                            .WhereBetween($"{prop.Key}.Верхняя граница", $"{prop.Value["B левая"]}", $"{prop.Value["B правая"]}")
                            .Select($"{viewName}.Нижняя граница as Полоса пропуск. (нижняя гр.)")
                            .Select($"{viewName}.Верхняя граница as Полоса пропуск. (верхняя гр.)");
                        break;

                    case "Показатели преломления":
                        JoinOneRangeCriteria(kataQuery, prop, "Значение показателя", "n",
                            ("Обозначение показателя", "Показат. преломл. (обозн.)"), ("Значение показателя", "Показат. преломл. (знач.)"));
                        break;

                    case "Коэффициенты линейного электрооптического эффекта":
                        viewName = PropertyNameToViewName(prop.Key);
                        kataQuery.Join($"{viewName}", $"Соединения.Номер соединения", $"{viewName}.Номер соединения")
                            .WhereLike($"{viewName}.Обозначение коэффициента", $"%{prop.Value["параметр"]}%")
                            .WhereBetween($"{prop.Key}.Значение коэффициента"
                                , $"{prop.Value[prop.Value["параметр"] + " левая"]}"
                                , $"{prop.Value[prop.Value["параметр"] + " правая"]}")
                            .Select($"{viewName}.Обозначение коэффициента as Лин. электроопт. (обозн. коэф.)")
                            .Select($"{viewName}.Значение коэффициента as Лин. электроопт. (знач. коэф.)");
                        break;

                    case "Нелинейные оптические коэффициенты":
                        JoinOneRangeCriteria(kataQuery, prop, "Значение коэффициента", "d",
                            ("Обозначение коэффициента", "Нелин. оптич. (обозн.)"), ("Значение коэффициента", "Нелин. оптич. (знач.)"));
                        break;

                    case "Компоненты тензора Миллера":
                        JoinOneRangeCriteria(kataQuery, prop, "Значение коэффициента", "delta",
                            ("Обозначение коэффициента", "Комп. тензора Миллера. (обозн.)"), ("Значение коэффициента", "Комп. тензора Миллера. (знач.)"));
                        break;

                    case "Пьезооптические и упругооптические коэффициенты":
                        if (prop.Value["параметр"] == "π")
                        {
                            prop.Value["параметр"] = "pi";
                        }
                        else
                        {
                            prop.Value["параметр"] = "p<";
                        }
                        viewName = PropertyNameToViewName(prop.Key);
                        kataQuery.Join($"{viewName}", $"Соединения.Номер соединения", $"{viewName}.Номер соединения")
                            .WhereLike($"{viewName}.Обозначение коэффициента", $"%{prop.Value["параметр"]}%")
                            .WhereBetween($"{prop.Key}.Значение коэффициента"
                                , $"{prop.Value[prop.Value["параметр"] + " левая"]}"
                                , $"{prop.Value[prop.Value["параметр"] + " правая"]}")
                            .Select($"{viewName}.Обозначение коэффициента as Пьезоопт. и упругоопт. (обозн.)")
                            .Select($"{viewName}.Значение коэффициента as Пьезоопт. и упругоопт. (знач.)");
                        break;

                    case "Коэффициент затухания (D) или скорость распространения (S) упругих волн":
                        viewName = PropertyNameToViewName(prop.Key);
                        kataQuery.Join($"{viewName}", $"Соединения.Номер соединения", $"{viewName}.Номер соединения");
                        if (prop.Value["параметр"] == "D")
                        {
                            kataQuery.WhereBetween($"{viewName}.Коэффициент затухания, дБ/см"
                                , $"{prop.Value[prop.Value["параметр"] + " левая"]}"
                                , $"{prop.Value[prop.Value["параметр"] + " правая"]}")
                            .Select($"{viewName}.Коэффициент затухания, дБ/см as Коэф. затухания, дБ/cм");
                        }
                        else if (prop.Value["параметр"] == "S")
                        {
                            kataQuery.WhereBetween($"{viewName}.Скорость волны, 10^5 см/сек"
                                , $"{prop.Value[prop.Value["параметр"] + " левая"]}"
                                , $"{prop.Value[prop.Value["параметр"] + " правая"]}")
                            .Select($"{viewName}.Скорость волны, 10^5 см/сек");
                        }
                        break;

                    case "Акустооптическая добротность":
                        viewName = PropertyNameToViewName(prop.Key);
                        kataQuery.Join($"{viewName}", $"Соединения.Номер соединения", $"{viewName}.Номер соединения");
                        string m1Column = "M1, 10-7 см2*cек/г";
                        string m2Column = "M2, 10-18 сек3/г";
                        string m3Column = "M3, 10-12 см*сек2/г";
                        if (prop.Value.ContainsKey("M1 левая"))
                        {
                            kataQuery.WhereRaw($"[{m1Column}] >= {prop.Value["M1 левая"]}");
                        }
                        if (prop.Value.ContainsKey("M1 правая"))
                        {
                            kataQuery.WhereRaw($"[{m1Column}] <= {prop.Value["M1 правая"]}");
                        }
                        if (prop.Value.ContainsKey("M2 левая"))
                        {
                            kataQuery.WhereRaw($"[{m2Column}] >= {prop.Value["M2 левая"]}");
                        }
                        if (prop.Value.ContainsKey("M2 правая"))
                        {
                            kataQuery.WhereRaw($"[{m2Column}] <= {prop.Value["M2 правая"]}");
                        }
                        if (prop.Value.ContainsKey("M3 левая"))
                        {
                            kataQuery.WhereRaw($"[{m3Column}] >= {prop.Value["M3 левая"]}");
                        }
                        if (prop.Value.ContainsKey("M3 правая"))
                        {
                            kataQuery.WhereRaw($"[{m3Column}] <= {prop.Value["M3 правая"]}");
                        }
                        // Select
                        if (prop.Value.ContainsKey("M1 левая") || prop.Value.ContainsKey("M1 правая"))
                        {
                            kataQuery.Select(m1Column);
                        }
                        if (prop.Value.ContainsKey("M2 левая") || prop.Value.ContainsKey("M2 правая"))
                        {
                            kataQuery.Select(m2Column);
                        }
                        if (prop.Value.ContainsKey("M3 левая") || prop.Value.ContainsKey("M3 правая"))
                        {
                            kataQuery.Select(m3Column);
                        }
                        break;
                }
            }

            // На случай, если нужно проверить результат построенного запроса.
            SqlResult result = compiler.Compile(kataQuery);
            string sql = result.Sql;

            // Достаются только формулы содинений.
            var chemicalsOnlyQuery = kataQuery.Clone();
            chemicalsOnlyQuery.Clauses.RemoveAll(clause => clause.Component == "select");
            chemicalsOnlyQuery.Select($"Соединения.Номер соединения as Id", $"Соединения.Соединение as HtmlName");
            List<ChemicalModel> briefChemicalsList = chemicalsOnlyQuery.Get()
                .Select(x => new ChemicalModel() { Id = x.Id, HtmlName = x.HtmlName }).ToList();
            // Сортировка формул соединений.
            briefChemicalsList.Sort(ChemicalModel.CompareChemicals);

            // Если критерий был всего один, можно выдать всю информацию из таблицы по нему.
            if (IsOnlyOneCriteria(properties))
            {
                kataQuery.Clauses.RemoveAll(clause => clause.Component == "select");
                kataQuery.Select($"Соединения.Соединение");
                kataQuery.Select($"{PropertyNameToViewName(properties.Keys.First())}.*");
            }

            return View("QueryResult",
                new QueryResultModel(briefChemicalsList,
                new CriteriaQueryTableModel(kataQuery.Get(), IsOnlyOneCriteria(properties))));
        }

        /// <summary>
        /// Возвращает представление с результатами многокритериального запроса.
        /// </summary>
        /// <returns></returns>
        [HttpGet("perform-query")]
        public IActionResult PerformQuery()
        {
            try
            {
                Dictionary<string, Dictionary<string, string>> properties = new();
                foreach (string key in Request.Query.Keys)
                {
                    // Добавляются только непустые вводимые значения.
                    if (Request.Query[key] != "")
                    {
                        string[] splitted = key.Split(':');
                        string property_name = splitted[0];
                        string param = splitted[1];
                        string value = Request.Query[key];

                        if (!properties.ContainsKey(property_name))
                        {
                            properties[property_name] = new Dictionary<string, string>();
                        }
                        properties[property_name].Add(param, value);
                    }
                }

                return MultipleCriteriaSearch(properties);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Вспомогательный метод для уменьшения дублирования кода.
        /// Подключает в общий запрос критерий с единственным параметром, входящим в определенный интервал.
        /// </summary>
        /// <param name="kataQuery"></param>
        /// <param name="prop"></param>
        /// <param name="columnBetweenName"></param>
        /// <param name="rangeName"></param>
        /// <param name="columnSelects"></param>
        [NonAction]
        private void JoinOneRangeCriteria(Query kataQuery, KeyValuePair<string, Dictionary<string, string>> prop,
            string columnBetweenName, string rangeName, params (string Name, string Alias)[] columnSelects)
        {
            string viewName = PropertyNameToViewName(prop.Key);
            kataQuery.Join($"{viewName}", $"Соединения.Номер соединения", $"{viewName}.Номер соединения")
                .WhereBetween($"{viewName}.{columnBetweenName}", $"{prop.Value[$"{rangeName} левая"]}", $"{prop.Value[$"{rangeName} правая"]}");
            foreach ((string Name, string Alias) columnName in columnSelects)
            {
                kataQuery.Select($"{viewName}.{columnName.Name} as {columnName.Alias}");
            }
        }
    }
}
