namespace Coursework_CrystalSite_Final.Models
{
    /// <summary>
    /// Модель аккаунта пользователя.
    /// </summary>
    class Account
    {
        /// <summary>
        /// Почта пользователя.
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        public string Password { get; set; }

        public Account() { }

        public Account(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
