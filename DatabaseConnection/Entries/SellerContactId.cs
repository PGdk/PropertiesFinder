namespace Models
{
    public class SellerContactId
    {
        public int Id { get; set; }
        public string Email { get; set; }

        public string Telephone { get; set; }

        /// <summary>
        /// Imię i nazwisko sprzedwcy
        /// </summary>
        public string Name { get; set; }
    }
}