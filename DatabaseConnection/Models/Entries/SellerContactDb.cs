namespace DatabaseConnection
{
    public class SellerContactDb
    {
        public long Id { get; set; }
        public string Email { get; set; }

        public string Telephone { get; set; }

        /// <summary>
        /// Imię i nazwisko sprzedwcy
        /// </summary>
        public string Name { get; set; }
    }
}