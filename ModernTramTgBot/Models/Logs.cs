namespace ModernTramTgBot.Models
{
    public class Logs
    {
        public int ID { get; set; }
        public DateTime ScheduledService { get; set; }
        public string RepairDescription { get; set; }
        public int TramID { get; set; }
        public int TechnicalStaff { get; set; }
    }
}
