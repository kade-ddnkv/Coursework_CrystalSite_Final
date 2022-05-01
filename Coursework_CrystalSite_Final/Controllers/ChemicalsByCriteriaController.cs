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

        private string propertyNameToViewName(string propertyName)
        {
            switch(propertyName)
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

        private IActionResult MultipleCriteriaSearch(Dictionary<string, Dictionary<string, string>> properties)
        {
            var connection = new SqlConnection(DatabaseConnection.connectionString);
            var compiler = new SqlServerCompiler();
            var db = new QueryFactory(connection, compiler);

            var kataQuery = db.Query("Соединения").Select($"Соединения.Соединение").Distinct();
            string viewName;
            foreach (var prop in properties)
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
                        viewName = propertyNameToViewName(prop.Key);
                        kataQuery.Join($"{viewName}", $"Соединения.Номер соединения", $"{viewName}.Номер соединения")
                            .Where($"{viewName}.Обозначение сингонии", $"{prop.Value["сингония"]}")
                            .Select($"{viewName}.Обозначение сингонии");
                        break;

                    case "Точечная группа":
                        viewName = propertyNameToViewName(prop.Key);
                        kataQuery.Join($"{viewName}", $"Соединения.Номер соединения", $"{viewName}.Номер соединения")
                            .Where($"{viewName}.Точечная группа симметрии", $"{prop.Value["тип"]}")
                            .Select($"{viewName}.Точечная группа симметрии");
                        break;

                    case "Тепловое расширение":
                        joinOneRangeCriteria(kataQuery, prop, "Значение коэффициента", "расширение",
                            ("Обозначение коэффициента", "Тепл. расшир. (обозн. коэф.)"), ("Значение коэффициента", "Тепл. расшир. (знач. коэф.)"));
                        break;

                    case "Теплопроводность":
                        joinOneRangeCriteria(kataQuery, prop, "Значение коэффициента", "теплопроводность",
                            ("Обозначение коэффициента", "Теплопр-ть (обозн. коэф.)"), ("Значение коэффициента", "Теплопр-ть (знач. коэф.)"));
                        break;

                    case "Диэлектрическая проницаемость":
                        joinOneRangeCriteria(kataQuery, prop, "Значение коэффициента", "проницаемость",
                            ("Обозначение коэффициента", "Диэл. прониц. (обозн. коэф.)"), ("Значение коэффициента", "Диэл. прониц. (знач. коэф.)"));
                        break;

                    case "Тангенс угла диэлектрических потерь":
                        joinOneRangeCriteria(kataQuery, prop, "Значение тангенса угла", "тангенс",
                            ("Обозначение тангенса угла потерь", "Обозначение тангенса угла потерь"), ("Значение тангенса угла", "Значение тангенса угла"));
                        break;

                    case "Пьезоэлектрические коэффициенты":
                        viewName = propertyNameToViewName(prop.Key);
                        kataQuery.Join($"{viewName}", $"Соединения.Номер соединения", $"{viewName}.Номер соединения")
                            .WhereLike($"{viewName}.Обозначение коэффициента", $"%{prop.Value["параметр"]}%")
                            .WhereBetween($"{prop.Key}.Значение коэффициента"
                                , $"{prop.Value[prop.Value["параметр"] + " левая"]}"
                                , $"{prop.Value[prop.Value["параметр"] + " правая"]}")
                            .Select($"{viewName}.Обозначение коэффициента as Пьезоэл. (обозн. коэф.)")
                            .Select($"{viewName}.Значение коэффициента as Пьезоэл. (знач. коэф.)");
                        break;

                    case "Коэффициенты электромеханической связи":
                        joinOneRangeCriteria(kataQuery, prop, "Значение коэффицента", "k",
                            ("Обозначение коэффициента", "Эл.-механ. связь (обозн. коэф.)"), ("Значение коэффицента", "Эл.-механ. связь (знач. коэф.)"));
                        break;

                    case "Упругие постоянные":
                        viewName = propertyNameToViewName(prop.Key);
                        kataQuery.Join($"{viewName}", $"Соединения.Номер соединения", $"{viewName}.Номер соединения")
                            .WhereLike($"{viewName}.Условия измерения", $"%{prop.Value["параметр"]}%")
                            .WhereBetween($"{prop.Key}.Обозначение коэффициента"
                                , $"{prop.Value[prop.Value["параметр"] + " левая"]}"
                                , $"{prop.Value[prop.Value["параметр"] + " правая"]}")
                            .Select($"{viewName}.Условия измерения as Упруг. пост. (параметр)")
                            .Select($"{viewName}.Обозначение коэффициента as Упруг. пост. (знач. коэф.)");
                        break;

                    case "Полоса пропускания":
                        viewName = propertyNameToViewName(prop.Key);
                        kataQuery.Join($"{viewName}", $"Соединения.Номер соединения", $"{viewName}.Номер соединения")
                            .WhereBetween($"{prop.Key}.Нижняя граница", $"{prop.Value["B левая"]}", $"{prop.Value["B правая"]}")
                            .WhereBetween($"{prop.Key}.Верхняя граница", $"{prop.Value["B левая"]}", $"{prop.Value["B правая"]}")
                            .Select($"{viewName}.Нижняя граница as Полоса пропуск. (нижняя гр.)")
                            .Select($"{viewName}.Верхняя граница as Полоса пропуск. (верхняя гр.)");
                        break;

                    case "Показатели преломления":
                        joinOneRangeCriteria(kataQuery, prop, "Значение показателя", "n",
                            ("Обозначение показателя", "Показат. преломл. (обозн.)"), ("Значение показателя", "Показат. преломл. (знач.)"));
                        break;

                    case "Коэффициенты линейного электрооптического эффекта":
                        viewName = propertyNameToViewName(prop.Key);
                        kataQuery.Join($"{viewName}", $"Соединения.Номер соединения", $"{viewName}.Номер соединения")
                            .WhereLike($"{viewName}.Обозначение коэффициента", $"%{prop.Value["параметр"]}%")
                            .WhereBetween($"{prop.Key}.Значение коэффициента"
                                , $"{prop.Value[prop.Value["параметр"] + " левая"]}"
                                , $"{prop.Value[prop.Value["параметр"] + " правая"]}")
                            .Select($"{viewName}.Обозначение коэффициента as Лин. электроопт. (обозн. коэф.)")
                            .Select($"{viewName}.Значение коэффициента as Лин. электроопт. (знач. коэф.)");
                        break;

                    case "Нелинейные оптические коэффициенты":
                        joinOneRangeCriteria(kataQuery, prop, "Значение коэффициента", "d",
                            ("Обозначение коэффициента", "Нелин. оптич. (обозн.)"), ("Значение коэффициента", "Нелин. оптич. (знач.)"));
                        break;

                    case "Компоненты тензора Миллера":
                        joinOneRangeCriteria(kataQuery, prop, "Значение коэффициента", "delta",
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
                        viewName = propertyNameToViewName(prop.Key);
                        kataQuery.Join($"{viewName}", $"Соединения.Номер соединения", $"{viewName}.Номер соединения")
                            .WhereLike($"{viewName}.Обозначение коэффициента", $"%{prop.Value["параметр"]}%")
                            .WhereBetween($"{prop.Key}.Значение коэффициента"
                                , $"{prop.Value[prop.Value["параметр"] + " левая"]}"
                                , $"{prop.Value[prop.Value["параметр"] + " правая"]}")
                            .Select($"{viewName}.Обозначение коэффициента as Пьезоопт. и упругоопт. (обозн.)")
                            .Select($"{viewName}.Значение коэффициента as Пьезоопт. и упругоопт. (знач.)");
                        break;

                    case "Коэффициент затухания (D) или скорость распространения (S) упругих волн":
                        viewName = propertyNameToViewName(prop.Key);
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
                }
            }

            // Достаются только формулы содинений.
            var chemicalsOnlyQuery = kataQuery.Clone();
            chemicalsOnlyQuery.Clauses.RemoveAll(clause => clause.Component == "select");
            chemicalsOnlyQuery.Select($"Соединения.Номер соединения as Id", $"Соединения.Соединение as HtmlName");
            List<ChemicalModel> briefChemicalsList = chemicalsOnlyQuery.Get()
                .Select(x => new ChemicalModel() { Id = x.Id, HtmlName = x.HtmlName }).ToList();
            // Сортировка формул соединений.
            briefChemicalsList.Sort(ChemicalModel.CompareChemicals);
            
            // Если критерий был всего один, можно выдать всю информацию из таблицы по нему.
            if (properties.Count == 1)
            {
                kataQuery.Clauses.RemoveAll(clause => clause.Component == "select");
                kataQuery.Select($"Соединения.Соединение");
                kataQuery.Select($"{propertyNameToViewName(properties.Keys.First())}.*");
            }

            // На случай, если нужно проверить результат построенного запроса.
            SqlResult result = compiler.Compile(kataQuery);
            string sql = result.Sql;

            return View("QueryResult",
                new QueryResultModel(briefChemicalsList,
                new DynamicTableModel(kataQuery.Get(), properties.Count == 1)));
        }

        public IActionResult PerformQuery()
        {
            try
            {
                Dictionary<string, Dictionary<string, string>> properties = new();
                foreach (string key in Request.Query.Keys)
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

                return MultipleCriteriaSearch(properties);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private void joinOneRangeCriteria(Query kataQuery, KeyValuePair<string, Dictionary<string, string>> prop,
            string columnBetweenName, string rangeName, params (string Name, string Alias)[] columnSelects)
        {
            string viewName = propertyNameToViewName(prop.Key);
            kataQuery.Join($"{viewName}", $"Соединения.Номер соединения", $"{viewName}.Номер соединения")
                .WhereBetween($"{viewName}.{columnBetweenName}", $"{prop.Value[$"{rangeName} левая"]}", $"{prop.Value[$"{rangeName} правая"]}");
            foreach ((string Name, string Alias) columnName in columnSelects)
            {
                kataQuery.Select($"{viewName}.{columnName.Name} as {columnName.Alias}");
            }
        }
    }
}
