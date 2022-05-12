using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace Coursework_CrystalSite_Final.Models
{
    /// <summary>
    /// Модель свойства из базы данных.
    /// </summary>
    public class PropertyModel
    {
        /// <summary>
        /// Номер свойства в БД.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Русское название свойства.
        /// </summary>
        public string NameRus { get; set; }

        /// <summary>
        /// Есть ли данные по свойству для выбранного соединения.
        /// </summary>
        public string Existing { get; set; }

        /// <summary>
        /// Отображение русского названия свойства в url-имя.
        /// </summary>
        public static Dictionary<string, string> NameRusToUrlName = new()
        {
            { "Аналитический обзор", "Analytical review" },
            { "Cостав соединения", "Compound composition" },
            { "Удельная теплоемкость", "Specific heat capacity" },
            { "Плотность", "Density" },
            { "Твердость", "Hardness" },
            { "Растворимость", "Solubility" },
            { "Температура плавления", "Melting point" },
            { "Температура Кюри", "Curie Temperature" },
            { "Характеристика кристаллической структуры", "Characteristics of the crystal structure" },
            { "Параметры элементарной ячейки", "Unit Cell Parameters" },
            { "Тепловое расширение", "Thermal expansion" },
            { "Теплопроводность", "Thermal conductivity" },
            { "Диэлектрическая проницаемость", "Dielectric constant" },
            { "Тангенс угла диэлектрических потерь", "The tangent of the dielectric loss angle" },
            { "Пьезоэлектрические коэффициенты", "Piezoelectric coefficients" },
            { "Коэффициенты электромеханической связи", "Electromechanical coupling coefficients" },
            { "Упругие постоянные", "Elastic constants" },
            { "Полоса пропускания", "Bandwidth" },
            { "Показатели преломления", "Refractive indices" },
            { "Коэффициенты Селмейера", "Selmeyer coefficients" },
            { "Коэффициенты линейного электрооптического эффекта", "Coefficients of the linear electro-optical effect" },
            { "Нелинейные оптические свойства", "Nonlinear optical properties" },
            { "Пьезооптические и упругооптические коэффициенты", "Piezo-optical and elastic-optical coefficients" },
            { "Распространение и затухание упругих волн", "Propagation and attenuation of elastic waves" },
            { "Акустооптические свойства", "Acousto-optic properties" },
            { "Литература", "Literature" },
        };

        /// <summary>
        /// Отображение url-имени свойства в номер в БД.
        /// </summary>
        public static Dictionary<string, int> UrlNameToId = new();

        /// <summary>
        /// Статический конструктор. Заполняет все отображения для свойств.
        /// </summary>
        static PropertyModel()
        {
            foreach (var pair in NameRusToUrlName)
            {
                NameRusToUrlName[pair.Key] = pair.Value.ToLower().Replace(" ", "-");
            }
            using IDbConnection db = new SqlConnection(DatabaseConnection.ConnectionString);
            List<PropertyModel> properties = db.Query<PropertyModel>(
                "SELECT [Номер свойства] as Id ,[Название свойства] as NameRus FROM dbo.[Свойства соединений]").ToList();
            foreach (PropertyModel property in properties)
            {
                string urlName = NameRusToUrlName[property.NameRus];
                UrlNameToId.Add(urlName, property.Id);
            }
        }
    }
}
