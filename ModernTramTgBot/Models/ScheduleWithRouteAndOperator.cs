namespace ModernTramTgBot.Models
{
    public class ScheduleWithRouteAndOperator
    {
        public int ID { get; set; }
        public int TramID { get; set; }
        public string Weekdays { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public string RouteName { get; set; }
        public decimal RoutLength { get; set; }
        public int Duration { get; set; }
        public string OperatorName { get; set; }
        public string OperatorTgName { get; set; }
        public List<Stops> Stops { get; set; }
    }
}
