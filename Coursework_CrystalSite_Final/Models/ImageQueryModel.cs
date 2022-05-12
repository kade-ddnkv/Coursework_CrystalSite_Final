namespace Coursework_CrystalSite_Final.Models
{
    /// <summary>
    /// Модель результата запроса графиков для определенного соединения для определенного свойства.
    /// </summary>
    public class ImageQueryModel
    {
        /// <summary>
        /// Html-имя текущего соединения.
        /// </summary>
        public string ChemicalFormula { get; set; }
        
        /// <summary>
        /// Название текущего свойства.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Список из графиков (моделей графиков).
        /// </summary>
        public List<ImageModel> Images { get; set; }
    }
}
