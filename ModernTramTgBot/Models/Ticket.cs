namespace ModernTramTgBot.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public DateTime PurchaseDateTime { get; set; }
        public DateTime ExpiryDateTime { get; set; }
        public decimal Price { get; set; }
        public int TgID { get; set; }
    }
}
