namespace ModernTramTgBot.Models
{
    public class Incident
    {
        public int ID { get; set; }
        public DateTime IncDateTime { get; set; }
        public string IncDescription { get; set; }
        public string IncStatus { get; set; }
        public int TramID { get; set; }
    }
}
