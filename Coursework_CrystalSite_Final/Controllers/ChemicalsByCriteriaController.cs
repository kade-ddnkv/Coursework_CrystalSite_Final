//using Dapper;
using Coursework_CrystalSite_Final.Models;
using Microsoft.AspNetCore.Mvc;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Coursework_CrystalSite_Final.Controllers
{
    public class ChemicalsByCriteriaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PerformQuery()
        {
            try
            {
                //List<(string property_name, string param, string value)> query = new();
                Dictionary<string, Dictionary<string, string>> property_groups = new();
                foreach (string key in Request.Query.Keys)
                {
                    string[] splitted = key.Split(':');
                    string property_name = splitted[0];
                    string param = splitted[1];
                    string value = Request.Query[key];

                    if (!property_groups.ContainsKey(property_name))
                    {
                        property_groups[property_name] = new Dictionary<string, string>();
                    }
                    property_groups[property_name].Add(param, value);
                }

                var connection = new SqlConnection(DatabaseConnection.connectionString);
                var compiler = new SqlServerCompiler();
                var db = new QueryFactory(connection, compiler);

                var kataQuery = db.Query("Соединения").Select($"Соединения.Соединение").Distinct();
                foreach (var prop in property_groups)
                {
                    switch (prop.Key)
                    {
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
                            kataQuery.Join($"{prop.Key}", $"Соединения.Номер соединения", $"{prop.Key}.Номер соединения")
                                .WhereBetween($"{prop.Key}.Моос", $"{prop.Value["твердость левая"]}", $"{prop.Value["твердость правая"]}")
                                .Select($"{prop.Key}.Моос");
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
                            kataQuery.Join($"Сингонии соединений", $"Соединения.Номер соединения", $"Сингонии соединений.Номер соединения")
                                .Where($"Сингонии соединений.Обозначение сингонии", $"{prop.Value["сингония"]}")
                                .Select($"Сингонии соединений.Обозначение сингонии");
                            break;

                        case "Тепловое расширение":
                            joinOneRangeCriteria(kataQuery, prop, "Значение коэффициента", "расширение", 
                                "Обозначение коэффициента", "Значение коэффициента");
                            break;

                        case "Теплопроводность":
                            joinOneRangeCriteria(kataQuery, prop, "Значение коэффициента", "теплопроводность",
                                "Обозначение коэффициента", "Значение коэффициента");
                            break;

                        case "Диэлектрическая проницаемость":
                            joinOneRangeCriteria(kataQuery, prop, "Значение коэффициента", "проницаемость",
                                "Обозначение коэффициента", "Значение коэффициента");
                            break;

                        case "Тангенс угла диэлектрических потерь":
                            joinOneRangeCriteria(kataQuery, prop, "Значение тангенса угла", "тангенс",
                                "Обозначение тангенса угла потерь", "Значение тангенса угла");
                            break;
                    }
                }
                var chemicalsOnlyQuery = kataQuery.Clone();
                chemicalsOnlyQuery.Clauses.RemoveAll(clause => clause.Component == "select");
                chemicalsOnlyQuery.Select($"Соединения.Соединение as HeadClue");
                return View("QueryResult",
                    new QueryResultModel(chemicalsOnlyQuery.Get().Select(x => x.HeadClue),
                    new DynamicTableModel(kataQuery.Get())));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private void joinOneRangeCriteria(Query kataQuery, KeyValuePair<string, Dictionary<string, string>> prop,
            string columnBetweenName, string rangeName, params string[] columnSelects)
        {
            kataQuery.Join($"{prop.Key}", $"Соединения.Номер соединения", $"{prop.Key}.Номер соединения")
                .WhereBetween($"{prop.Key}.{columnBetweenName}", $"{prop.Value[$"{rangeName} левая"]}", $"{prop.Value[$"{rangeName} правая"]}");
            foreach (string columnName in columnSelects)
            {
                kataQuery.Select($"{prop.Key}.{columnName}");
            }
        }
    }
}
