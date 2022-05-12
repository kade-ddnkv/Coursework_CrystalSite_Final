namespace Coursework_CrystalSite_Final.Models
{
    /// <summary>
    /// Модель библиографической ссылки.
    /// </summary>
    public class BookModel
    {
        /// <summary>
        /// Номер библиографической ссылки.
        /// </summary>
        public int BookNumber { get; set; }

        /// <summary>
        /// Авторы книги/статьи.
        /// </summary>
        public string Authors { get; set; }

        /// <summary>
        /// Название литературы и страницы, на которых расположен нужный материал.
        /// </summary>
        public string BookNameAndPages { get; set; }

        /// <summary>
        /// Название статьи.
        /// </summary>
        public string ArticleName { get; set; }

        /// <summary>
        /// Ссылка на просмотр/скачивание статьи.
        /// </summary>
        public string DoiLink { get; set; }
    }
}
